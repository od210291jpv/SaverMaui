﻿// This file was auto-generated by ML.NET Model Builder. 
using Microsoft.ML.Data;

namespace ImageRecognitionNN
{
    public partial class ImageLearning
    {

        /// <summary>
        /// model output class for ImageLearning.
        /// </summary>
        public class ModelOutput
        {
            [ColumnName(@"Label")]
            public uint Label { get; set; }

            [ColumnName(@"ImageSource")]
            public byte[] ImageSource { get; set; }

            [ColumnName(@"PredictedLabel")]
            public string PredictedLabel { get; set; }

            [ColumnName(@"Score")]
            public float[] Score { get; set; }

        }
    }
}
