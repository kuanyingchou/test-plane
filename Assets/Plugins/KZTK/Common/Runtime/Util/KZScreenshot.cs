using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//[ modified from http://docs.unity3d.com/ScriptReference/Texture2D.ReadPixels.html
public class KZScreenshot : MonoBehaviour 
{

    // Grab a screen shot and upload it to a CGI script.
    // The CGI script must be able to hande form uploads.
    public string uploadURL = "http://192.168.0.92:8000/upload";
    public int bufferSize = 10;
    public bool enableTimelapse;
    public float timelapseInitialDelay = 0;
    public float timelapseInterval = 10;

    Queue<Screenshot> buffer;
    int width;
    int height;

    //private class Buffer<T>
    //{
    //    Texture2D[] slots;
    //    int itemCount = 0;
    //    int startIndex = 0;
    //    int width;
    //    int height;

    //    public Buffer(int size, int width, int height)
    //    {
    //        slots = new Texture2D[size];
    //        for (int i = 0; i < size; i++)
    //        {
    //            slots[i] = new Texture2D(width, height, TextureFormat.RGB24, false);
    //        }
    //        this.width = width;
    //        this.height = height;
    //    }
        
    //    public void Push(Texture2D tex)
    //    {
    //        if (IsFull())
    //        {
    //            Debug.LogError("buffer is full!");
    //        }
    //        else
    //        {
    //            slots[(startIndex + itemCount) % slots.Length] = tex;
    //        }
    //    }

    //    private bool IsFull()
    //    { 
    //        return (startIndex + itemCount + 1) % slots.Length == startIndex;
    //    }

    //    public int Count()
    //    {
    //        return itemCount;
    //    }

    //    public Texture2D Pop()
    //    {
    //        if (itemCount == 0)
    //        {
    //            Debug.LogError("nothing to pop!");
    //            return null;
    //        }
    //        Texture2D tex = slots[startIndex];
    //        startIndex = (startIndex + 1) % slots.Length;
    //        itemCount -= 1;
    //        return tex;
    //    }
    //}

    struct Screenshot
    {
        public string name;
        public Texture2D texture;

        public Screenshot(string n, Texture2D tex)
        {
            name = n;
            texture = tex;
        }
    }

    public void Start()
    {
        width = Screen.width;
        height = Screen.height;
        buffer = new Queue<Screenshot>(bufferSize);
        //texture.Add(new Texture2D(width, height, TextureFormat.RGB24, false);

        //StartCoroutine(CaptureAndDetectError());

        if (enableTimelapse)
        {
            StartCoroutine(Repeat(() =>
            {
                StartCoroutine(Capture());

            }, timelapseInitialDelay, timelapseInterval));
        }

        StartCoroutine(UploadFiles());
    }

    public IEnumerator Capture()
    {
        if (buffer.Count >= bufferSize)
        {
            Debug.LogWarning("buffer is full!");
            yield break;
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        yield return StartCoroutine(CaptureScreenshot(texture));

        string filename = 
                string.Format("screenshot_frame-{0}_{1:yyyy-M-d_HH-mm-ss}.png", 
                Time.frameCount, System.DateTime.Now);

        buffer.Enqueue(new Screenshot(filename, texture));

        Debug.Log(filename + " captured!");

        //Destroy(texture); //wait till upload is done
    }

    public IEnumerator Capture(int frameCount)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return StartCoroutine(Capture());
        }
    }

    private bool IsShaderOK(Texture2D texture, int threshold)
    {
        int pixelCount = GetTotalPixel(texture, Color.magenta);
        if (pixelCount > threshold)
        {
            Debug.LogError(pixelCount + " pixels are in magenta!");
            return false;
        }
        else
        {
            return true;
        }
    }

    private int GetTotalPixel(Texture2D texture, Color color)
    {
        int pixelCount = 0;
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y) == Color.magenta)
                {
                    pixelCount++;
                }
            }
        }
        return pixelCount;
    }

    private IEnumerator Repeat(System.Action action, float firstDelay, float delay)
    {
        yield return new WaitForSeconds(firstDelay);
        while(true)
        {
            action();
            yield return new WaitForSeconds(delay);
        }
        
    }

    
    private IEnumerator CaptureScreenshot(Texture2D texture)
    {
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();

        // Read screen contents into the texture
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        //[ System.Drawing is not supported in Unity
        //System.Drawing.Image.FromFile(Application.persistentDataPath + "/" + filename); 
    }

    private IEnumerator UploadFiles()
    {
        while(true)
        {
            if (buffer.Count > 0)
            {
                Screenshot shot = buffer.Dequeue();
                yield return StartCoroutine(
                            UploadFile(shot.texture.EncodeToPNG(), shot.name, uploadURL));
                Destroy(shot.texture);
                Debug.Log(shot.name + " uploaded!");
            }
            else 
            {
                yield return null;
            }
        }
        
    }

    private IEnumerator UploadFile(
            byte[] bytes, string filename, string url, 
            string fieldname = "file", string type="image/png")
    {
        // Create a Web Form
        var form = new WWWForm();
        form.AddBinaryData(fieldname, bytes, filename, type);

        // Upload to a cgi script
        var w = new WWW(url, form);
        yield return w;  
        if (!string.IsNullOrEmpty(w.error))
            print(w.error);
        else
            print("finished uploading " + filename);
    }

    //private bool CaptureScreenshot(string filename)
    //{
    //    Application.CaptureScreenshot(filename);
    //    Debug.Log("Shoot!");
    //    bool success = File.Exists(Path.Combine(Application.persistentDataPath, filename));
    //    return success;
    //}
    //private Texture2D ReadScreenshot(string filename)
    //{
    //    filename = Path.Combine(Application.persistentDataPath, filename);
    //    byte[] bytes = File.ReadAllBytes(filename);
    //    Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, mipmap: false);
    //    texture.LoadImage(bytes);
    //    return texture;
    //}

}
