using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TiffLibrary;
using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using TiffLibrary.PixelFormats;

public class TiffReader : MonoBehaviour
{
    public RawImage rawImage;  // Sleep je RawImage hier in de Unity Inspector
    private string downloadedFilePath;

    public async void GetTexture(string well)
    {
        downloadedFilePath = await GetBlob(well);
        Debug.Log($"TIFF downloaded to: {downloadedFilePath}");

        if (!string.IsNullOrEmpty(downloadedFilePath))
        {
            string jpgPath = MakeTile(0, 0, 410, 301, downloadedFilePath);
            ApplyTextureToRawImage(jpgPath);
        }
        else
        {
            Debug.LogError("Failed to download TIFF file.");
        }
    }

    public string MakeTile(int px, int py, int lxx, int lyy, string filePath)
    {
        using TiffFileReader tiff = TiffFileReader.Open(filePath);
        TiffImageFileDirectory ifd = tiff.ReadImageFileDirectory();
        TiffImageDecoder decoder = tiff.CreateImageDecoder(ifd);

        TiffRgba32[] pixels = new TiffRgba32[lxx * lyy];
        TiffMemoryPixelBuffer<TiffRgba32> pixelBuffer = new TiffMemoryPixelBuffer<TiffRgba32>(pixels, lxx, lyy, writable: true);
        decoder.Decode(new TiffPoint(px, py), pixelBuffer);

        // Bereken min en max helderheid
        byte min = 255, max = 0;
        foreach (var pixel in pixels)
        {
            byte brightness = Math.Max(pixel.R, Math.Max(pixel.G, pixel.B));
            if (brightness < min) min = brightness;
            if (brightness > max) max = brightness;
        }

        float contrastFactor = 255f / (max - min);

        // Pas contrast aan
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].R = (byte)Mathf.Clamp((pixels[i].R - min) * contrastFactor, 0, 255);
            pixels[i].G = (byte)Mathf.Clamp((pixels[i].G - min) * contrastFactor, 0, 255);
            pixels[i].B = (byte)Mathf.Clamp((pixels[i].B - min) * contrastFactor, 0, 255);
        }

        // Maak Texture2D en sla op als JPG
        Texture2D texture = new Texture2D(lxx, lyy, TextureFormat.RGBA32, false);
        for (int y = 0; y < lyy; y++)
        {
            int flippedY = lyy - 1 - y;
            for (int x = 0; x < lxx; x++)
            {
                TiffRgba32 pixel = pixels[flippedY * lxx + x];
                texture.SetPixel(x, y, new Color32(pixel.R, pixel.G, pixel.B, pixel.A));
            }
        }
        texture.Apply();

        string jpgPath = Path.Combine(Application.temporaryCachePath, "output.jpg");
        File.WriteAllBytes(jpgPath, texture.EncodeToJPG());

        Debug.Log($"Saved JPG to {jpgPath}");
        return jpgPath;
    }

    public async Task<string> GetBlob(string well)
    {
        string connectionString = "...";
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("bronze");

        BlobClient blobClient = containerClient.GetBlobClient($"FIS241015130719_T0-T7/WELL_{well}_T5_C0.tif");
        string localFilePath = Path.Combine(Application.temporaryCachePath, $"WELL_{well}_T5_C0.tif");

        Debug.Log("Downloading blob...");

        using (var fileStream = File.OpenWrite(localFilePath))
        {
            await blobClient.DownloadToAsync(fileStream);
        }

        Debug.Log($"Blob downloaded to: {localFilePath}");
        return localFilePath;
    }

    private void ApplyTextureToRawImage(string imagePath)
    {
        if (rawImage == null)
        {
            Debug.LogError("RawImage is not assigned in the Inspector!");
            return;
        }

        byte[] imageData = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        rawImage.texture = texture;

        Debug.Log("Texture applied to RawImage.");
    }
}
