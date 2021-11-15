namespace OpenCvSharp.Demo
{
    using UnityEngine;
    using OpenCvSharp;
    using System.Collections.Generic;

    public class FaceTracker : WebCamera
    {
        [Header("Imported Data")]
        public TextAsset faces;
        public TextAsset eyes;
        public TextAsset shapes;

        [Header("Face marks duplication")]
        public GameObject markPlaceholder;
        public int numMarks = 69;
        public List<Transform> placeholders;

        [Header("Face marks indexes")]
        public int noseMin = 30;
        public int noseBridgTop = 25;
        public int noseBridgBot = 29;
        public int noseMax = 35;
        public int rEyeMin = 35;
        public int rEyeMax = 41;
        public int lEyeMin = 41;
        public int lEyeMax = 47;

        [Header("Left Eye Trackers")]
        public Transform leftTopEyeTracker;
        public Transform leftCentEyeTracker;
        public Transform leftBotEyeTracker;

        [Header("Right Eye Trackers")]
        public Transform rightTopEyeTracker;
        public Transform rightCentEyeTracker;
        public Transform rightBotEyeTracker;

        [Header("Nose Trackers")]
        public Transform noseBridgTopTracker;
        public Transform noseBridgBotTracker;
        public Transform noseTracker;

        //[Header("Center Trackers")]
        //public Transform centerTracker;

        FaceProcessorLive<WebCamTexture> processor;


        protected override void Awake()
        {
            base.Awake();
            base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

            
            processor = new FaceProcessorLive<WebCamTexture>();
            processor.Initialize(faces.text, eyes.text, shapes.bytes);

            // data stabilizer - affects face rects, face landmarks etc.
            processor.DataStabilizer.Enabled = true;        // enable stabilizer
            processor.DataStabilizer.Threshold =1.0;       // threshold value in pixels
            processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

            // performance data - some tricks to make it work faster
            processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
            processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
        }

        private void Start()
        {
            placeholders.Clear();
            placeholders.Add(markPlaceholder.transform);

            for (int i = 0; i < numMarks; i++)
            {
                var go = GameObject.Instantiate(markPlaceholder);
                go.transform.parent = markPlaceholder.transform.parent;
                placeholders.Add(go.transform);
            }
        }


        protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
        {
            // detect everything we're interested in
            processor.ProcessTexture(input, TextureParameters);

            // mark detected objects
            processor.MarkDetected();
            if (processor.Faces.Count > 0)
            {

                var marks = processor.Faces[0].Marks;


                for (int i = 0; i < processor.Faces[0].Marks.Length; i++)
                    placeholders[i].transform.position = new Vector3(marks[i].X, -marks[i].Y, 0);
                //placeholders[i].transform.localPosition = placeholders[i].InverseTransformPoint(new Vector3(marks[i].X, -marks[i].Y, 0));

                var nosePoint = AveragePoint(marks, noseMin, noseMax);
                var noseBridgTopPoint = marks[noseBridgTop];
                var noseBridgBotPoint = marks[noseBridgBot];

                var rEyeTopPoint = AveragePoint(marks, 38, 40);
                var rEyePoint = AveragePoint(marks, rEyeMin, rEyeMax);
                var rEyeBotPoint = AveragePoint(marks, 41, 43);

                var lEyeTopPoint = AveragePoint(marks, 44, 46);
                var lEyePoint = AveragePoint(marks, lEyeMin, lEyeMax);
                var lEyeBotPoint = AveragePoint(marks, 47, 49);

                rightTopEyeTracker.position = new Vector3(rEyeTopPoint.x, -rEyeTopPoint.y, 0);
                rightCentEyeTracker.position = new Vector3(rEyePoint.x, -rEyePoint.y, 0);
                rightBotEyeTracker.position = new Vector3(rEyeBotPoint.x, -rEyeBotPoint.y, 0);

                leftTopEyeTracker.position = new Vector3(lEyePoint.x, -lEyePoint.y, 0);
                leftCentEyeTracker.position =new Vector3(lEyeTopPoint.x, -lEyeTopPoint.y, 0);
                leftBotEyeTracker.position =new Vector3(lEyeBotPoint.x, -lEyeBotPoint.y, 0);

                noseTracker.position = new Vector3(nosePoint.x, -nosePoint.y, 0);
                noseBridgTopTracker.position = new Vector3(noseBridgTopPoint.X, -noseBridgTopPoint.Y, 0);
                noseBridgBotTracker.position = new Vector3(noseBridgBotPoint.X, -noseBridgBotPoint.Y, 0);

            }

            // processor.Image now holds data we'd like to visualize
            output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

            return true;
        }


        #region Utils
    

        Vector2 AveragePoint(Point[] points, int startingIndex, int endingIndex)
        {
            var numPoints = endingIndex - startingIndex;
            if (endingIndex > points.Length - 1)
            {
                //Debug.LogError($"Index would be out of range: {endingIndex}");
                return Vector2.zero;
            }

            Vector2 sumVector = Vector2.zero;
            for (int i = startingIndex; i < endingIndex; i++)
                sumVector += new Vector2(points[i].X, points[i].Y);

            return new Vector2(sumVector.x / numPoints, sumVector.y / numPoints);
        }
    }
    #endregion Utils
}