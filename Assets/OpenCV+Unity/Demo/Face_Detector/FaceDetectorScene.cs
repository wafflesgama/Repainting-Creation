namespace OpenCvSharp.Demo
{
    using System;
    using System.Linq;
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using OpenCvSharp;
    using System.Threading.Tasks;

    public class FaceDetectorScene : WebCamera
    {
        public TextAsset faces;
        public TextAsset eyes;
        public TextAsset shapes;
        public List<GameObject> placeholders;
        public List<GameObject> placeholders2;
        public List<Renderer> objectsToChangeMat;
        public Material altMat1;
        public Material altMat2;
        public int materialIndex1 = 20;
        public int materialIndex2 = 30;
        public Material objectMat1;
        public Material objectMat2;
        public int noseMin = 30;
        public int noseMax = 35;
        public int rEyeMin = 35;
        public int rEyeMax = 41;
        public int lEyeMin = 41;
        public int lEyeMax = 47;
        public Vector3 offsetPos;
        public Transform camera;
        public Vector3 cameraoffset;
        public float moveSpeed;
        public float lerpSpeed;
        public float rotSpeed;
        public float rotLerpSpeed;
        public float rotOffset = 90;
        public int blinkCounter = 0;
        public float blinkSpeed = 2;
        public float minEyeDist = 2.5f;
        public float closeMinEyeDist = 3.5f;
        Quaternion rotTargeted;
        Vector3 movTargeted;
        bool blinkFlag = true;
        bool materialFlag;
        DateTime blinkTime = DateTime.Now;

        private FaceProcessorLive<WebCamTexture> processor;

        /// <summary>
        /// Default initializer for MonoBehavior sub-classes
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

            byte[] shapeDat = shapes.bytes;
            if (shapeDat.Length == 0)
            {
                string errorMessage =
                    "In order to have Face Landmarks working you must download special pre-trained shape predictor " +
                    "available for free via DLib library website and replace a placeholder file located at " +
                    "\"OpenCV+Unity/Assets/Resources/shape_predictor_68_face_landmarks.bytes\"\n\n" +
                    "Without shape predictor demo will only detect face rects.";

#if UNITY_EDITOR
                // query user to download the proper shape predictor
                if (UnityEditor.EditorUtility.DisplayDialog("Shape predictor data missing", errorMessage, "Download", "OK, process with face rects only"))
                    Application.OpenURL("http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2");
#else
             UnityEngine.Debug.Log(errorMessage);
#endif
            }

            processor = new FaceProcessorLive<WebCamTexture>();
            processor.Initialize(faces.text, eyes.text, shapes.bytes);

            // data stabilizer - affects face rects, face landmarks etc.
            processor.DataStabilizer.Enabled = true;        // enable stabilizer
            processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
            processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

            // performance data - some tricks to make it work faster
            processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
            processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
        }


        private void Start()
        {
            for (int i = 0; i < 69; i++)
            {
                var go = GameObject.Instantiate(placeholders[0]);
                go.transform.parent = placeholders[0].transform.parent;
                if (i > materialIndex1)
                {
                    if (i > materialIndex2)
                        go.GetComponent<Renderer>().material = altMat2;
                    else
                        go.GetComponent<Renderer>().material = altMat1;
                }
                placeholders.Add(go);
            }
        }



        /// <summary>
        /// Per-frame video capture processor
        /// </summary>
        protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
        {
            // detect everything we're interested in
            processor.ProcessTexture(input, TextureParameters);

            // mark detected objects
            processor.MarkDetected();
            if (processor.Faces.Count > 0)
            {
                //Debug.Log(processor.Faces.Count + " Faces");

                //Debug.Log(processor.Faces[0].Marks.Length + " marks");

                for (int i = 0; i < processor.Faces[0].Marks.Length; i++)
                {

                    //skipCount++;
                    //if (skipCount > skipMarks) skipCount = 0;
                    //if (skipCount != 0) continue;

                    //if (placeholders.Count - 1 < placedCount) break;
                    var marks = processor.Faces[0].Marks;
                    placeholders[i].transform.position = offsetPos + new Vector3(marks[i].X, -marks[i].Y, 0);
                    //placedCount++;
                }


                var nosePoint = AveragePoint(processor.Faces[0].Marks, noseMin, noseMax);
                var rEyePoint = AveragePoint(processor.Faces[0].Marks, rEyeMin, rEyeMax);
                var lEyePoint = AveragePoint(processor.Faces[0].Marks, lEyeMin, lEyeMax);
                var rEyeTopPoint = AveragePoint(processor.Faces[0].Marks, 38, 40);
                var rEyeBotPoint = AveragePoint(processor.Faces[0].Marks, 41, 43);
                var lEyeTopPoint = AveragePoint(processor.Faces[0].Marks, 44, 46);
                var lEyeBotPoint = AveragePoint(processor.Faces[0].Marks, 47, 49);

                placeholders2[0].transform.position = offsetPos + new Vector3(nosePoint.x, -nosePoint.y, 0);
                placeholders2[1].transform.position = offsetPos + new Vector3(rEyePoint.x, -rEyePoint.y, 0);
                placeholders2[2].transform.position = offsetPos + new Vector3(lEyePoint.x, -lEyePoint.y, 0);

                movTargeted = Centerof3(placeholders2[0].transform.position, placeholders2[1].transform.position, placeholders2[2].transform.position);

                placeholders2[3].transform.position = movTargeted;

                placeholders2[4].transform.position = offsetPos + new Vector3(rEyeTopPoint.x, -rEyeTopPoint.y, 0);
                placeholders2[5].transform.position = offsetPos + new Vector3(rEyeBotPoint.x, -rEyeBotPoint.y, 0);
                placeholders2[6].transform.position = offsetPos + new Vector3(lEyeTopPoint.x, -lEyeTopPoint.y, 0);
                placeholders2[7].transform.position = offsetPos + new Vector3(lEyeBotPoint.x, -lEyeBotPoint.y, 0);

                var rEyeDist = Vector2.Distance(rEyeTopPoint, rEyeBotPoint);
                var lEyeDist = Vector2.Distance(lEyeTopPoint, lEyeBotPoint);



                var eyeLineDir = rEyePoint - lEyePoint;
                rotTargeted = Quaternion.Euler(camera.eulerAngles.x, camera.eulerAngles.y, rotOffset - Vector3.Angle(Vector3.up, eyeLineDir) * rotSpeed);
                if (rEyeDist < minEyeDist && lEyeDist < closeMinEyeDist || rEyeDist < closeMinEyeDist && lEyeDist < minEyeDist)
                {
                    var blinkDif = DateTime.Now - blinkTime;

                    if (blinkDif.TotalSeconds > blinkSpeed)
                    {
                        blinkTime = DateTime.Now;
                        Debug.Log($"R eye dist: {rEyeDist} || L eye dist: {lEyeDist}");
                        blinkFlag = false;
                        blinkCounter++;

                        foreach (var item in objectsToChangeMat)
                        {
                            item.material = materialFlag ? objectMat1 : objectMat2;
                        }
                        materialFlag = !materialFlag;
                        //BlinkWait();
                    }
                }

                //Debug.Log(Vector3.Angle(Vector3.up, eyeLineDir));
                //camera.eulerAngles = new Vector3(camera.eulerAngles.x, camera.eulerAngles.y, rotOffset - Vector3.Angle(Vector3.up, eyeLineDir)*rotSpeed);
                //camera.eulerAngles = Vector3.LerpUnclamped(camera.eulerAngles, new Vector3(camera.eulerAngles.x, camera.eulerAngles.y, rotOffset - Vector3.Angle(Vector3.up, eyeLineDir) * rotSpeed), Time.deltaTime * rotLerpSpeed);
            }

            camera.position = Vector3.Lerp(camera.position, movTargeted * moveSpeed + cameraoffset, Time.deltaTime * lerpSpeed);
            camera.rotation = Quaternion.Lerp(camera.rotation, rotTargeted, Time.deltaTime * rotLerpSpeed);


            // processor.Image now holds data we'd like to visualize
            output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

            return true;
        }




        Vector3 Centerof3(Vector3 p1, Vector3 p2, Vector3 p3) => new Vector3((p1.x + p2.x + p3.x) / 3, (p1.y + p2.y + p3.y) / 3, (p1.z + p2.z + p3.z) / 3);

        Vector2 AveragePoint(Point[] points, int startingIndex, int endingIndex)
        {
            var numPoints = endingIndex - startingIndex;
            Vector2 sumVector = Vector2.zero;
            for (int i = startingIndex; i < endingIndex; i++)
                sumVector += new Vector2(points[i].X, points[i].Y);

            return new Vector2(sumVector.x / numPoints, sumVector.y / numPoints);
        }
    }
}