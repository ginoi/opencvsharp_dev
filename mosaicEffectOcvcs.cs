using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace mosaicEffectOcvcs
{
    class Program
    {

        /*
        *  width in byte is (width) x (nChannel), (widthStep) is stride 
        *  640x480 640/40 = 16 ,480/40 = 12, 40 = hdiv,wdiv
        *  
        *     (0,0)                               (width,0)
        *     +--------+-- --+--------+----- -----+
        *     |  0,0   |     | i,0    |           |
        *     |        |  pij(0,0)    |           | 
        *     +--------+-- --+--------+----    ---+
        *     |  0,j   |     | i,j    |           |
        *     |        |     |        |           |
        *     +--------+-- --+--------+----    ---+
        *     |        |     |        |           |
        *
        *     |        |     |        |           |
        *     +--------+-- --+--------+----    ---+
        *     |  0,J-1 |     |        |           |
        *     |        |     |        |           |
        *     +--------+-- --+--------+----    ---+
        * pij (0,0) = (j * height/hdiv ) * width + (i * width/wdiv)     
        * pij (x,y) = (j * height/hdiv + y) * width + (i * width/wdiv + x), 0 <= x < width/wdiv, 0<= y < height/hdiv 
        *     
        * */
        static void Mosaic(IplImage dst, IplImage src, int width, int height, bool enable)
        {
            int wdiv = src.Width / width;
            int hdiv = src.Height / height;

            //System.Console.WriteLine("wdiv ={0} \n", wdiv.ToString());
            //System.Console.WriteLine("hdiv ={0} \n", hdiv.ToString());

            unsafe
            {
                byte* p = (byte*)src.ImageData;
                byte* q = (byte*)dst.ImageData;
                for (int j = 0; j < hdiv; j++)
                {
                    for (int i = 0; i < wdiv; i++)
                    {
                        if (enable)
                        {
                            int r, g, b;
                            r = g = b = 0;

                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    r += p[(j * src.Height / hdiv + y) * src.WidthStep + (i * src.WidthStep / wdiv) + x * src.NChannels];
                                    g += p[(j * src.Height / hdiv + y) * src.WidthStep + (i * src.WidthStep / wdiv) + x * src.NChannels + 1];
                                    b += p[(j * src.Height / hdiv + y) * src.WidthStep + (i * src.WidthStep / wdiv) + x * src.NChannels + 2];
                                }
                            }

                            r = (r / (height * width) > 255) ? 255 : r / (height * width);
                            g = (g / (height * width) > 255) ? 255 : g / (height * width);
                            b = (b / (height * width) > 255) ? 255 : b / (height * width);

                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    q[(j * dst.Height / hdiv + y) * dst.WidthStep + (i * dst.WidthStep / wdiv) + x * dst.NChannels] = (byte)r;
                                    q[(j * dst.Height / hdiv + y) * dst.WidthStep + (i * dst.WidthStep / wdiv) + x * dst.NChannels + 1] = (byte)g;
                                    q[(j * dst.Height / hdiv + y) * dst.WidthStep + (i * dst.WidthStep / wdiv) + x * dst.NChannels + 2] = (byte)b;
                                }
                            }
                        }
                        else
                        {// Just copy
                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    q[(j * dst.Height / hdiv + y) * dst.WidthStep + (i * dst.WidthStep / wdiv) + x * dst.NChannels]
                                            = p[(j * src.Height / hdiv + y) * src.WidthStep + (i * src.WidthStep / wdiv) + x * src.NChannels];
                                    q[(j * dst.Height / hdiv + y) * dst.WidthStep + (i * dst.WidthStep / wdiv) + x * dst.NChannels + 1]
                                            = p[(j * src.Height / hdiv + y) * src.WidthStep + (i * src.WidthStep / wdiv) + x * src.NChannels + 1];
                                    q[(j * dst.Height / hdiv + y) * dst.WidthStep + (i * dst.WidthStep / wdiv) + x * dst.NChannels + 2]
                                            = p[(j * src.Height / hdiv + y) * src.WidthStep + (i * src.WidthStep / wdiv) + x * src.NChannels + 2];
                                }
                            }
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {

            // Assumpting the num of camera is one
            using (var cam = Cv.CreateCameraCapture(-1))
            {
                IplImage src, dst;
                bool enable = true;
                //TODO               
                // We might need to configure camera resolution
                /*
                double w = 1280, h = 960;
                Cv.SetCaptureProperty(cam, CaptureProperty.FrameWidth, w);
                Cv.SetCaptureProperty(cam, CaptureProperty.FrameHeight, h);
                */

                // Query once to get camera property and alloc dst image
                src = cam.QueryFrame();
                dst = Cv.CreateImage(Cv.Size(src.Width, src.Height), BitDepth.U8, src.NChannels);

                // loop capturing and exit if esc is pressed
                while (true)
                {
                    // Capture frame
                    src = cam.QueryFrame();

                    // Process mosaic with C style
                    Mosaic(dst, src, 16, 12, enable);

                    // Display process
                    Cv.ShowImage("cam", dst);
                    int c = Cv.WaitKey(1);
                    
                    // if you press ESC, abort this application
                    if (c == '\x1b')
                        break;
                    // if you press 'e', switch between mosaic and copy
                    if (c == 'e')
                        enable ^= true;
                }
                Cv.DestroyWindow("cam");
            }
        }
    }
}
