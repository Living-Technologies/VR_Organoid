using System;
using System.Collections.Generic;
using UnityEngine;
using BitMiracle.LibTiff.Classic;
using Numpy;

public class TiffShapeViewer : MonoBehaviour
{
    public string tiffFilePath = "Assets/input.tif"; // Path to the TIFF file

    void Start()
    {
        DisplayTiffShape(tiffFilePath);
    }

    void DisplayTiffShape(string tiffFile)
    {
        // Read TIFF data into a NumPy array
        var tiffData = LoadTiffAsNumPyArray(tiffFile);

        if (tiffData == null)
        {
            Debug.LogError("Failed to load TIFF data.");
            return;
        }

        // Print the shape of the NumPy array
        Debug.Log($"TIFF Data Shape: {tiffData.shape}");
    }

    NDarray LoadTiffAsNumPyArray(string tiffFile)
    {
        List<NDarray> frames = new List<NDarray>();

        using (Tiff image = Tiff.Open(tiffFile, "r"))
        {
            if (image == null)
            {
                Debug.LogError($"Could not open TIFF file: {tiffFile}");
                return null;
            }

            int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            int depth = image.NumberOfDirectories();

            for (int frame = 0; frame < depth; frame++)
            {
                image.SetDirectory((short)frame);

                byte[] buffer = new byte[width * height];
                image.ReadRawStrip(0, buffer, 0, buffer.Length);

                // Convert buffer to a NumPy array
                var slice = np.array(buffer).reshape(height, width);
                frames.Add(slice);
            }
        }

        // Convert List<NDarray> to NDarray[]
        return np.stack(frames.ToArray(), axis: 0); // Shape will be (time, height, width)
    }
}
