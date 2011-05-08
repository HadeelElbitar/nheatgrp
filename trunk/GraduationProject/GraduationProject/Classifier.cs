﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using openCV;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.ML;
using Emgu.CV.ML.Structure;

namespace GraduationProject
{
    class Classifier
    {
        //MCvBGStatModel yy;
        //MCvFGDStatModelParams ii;

        //Matrix<int> em = new Matrix<int>(1, 1);
        //EMParams em = new EMParams();
        public Classifier() { }
        public void EM()
        {
            //EMTEST.EMTET Em = new EMTEST.EMTET();
            //Em.Trainn();
            Random r = new Random(DateTime.Now.Millisecond);
            int N = 2000;
            int D = 10;
            int G = 10;

            EM Em = new EM();
            Matrix<int> labels = new Matrix<int>(N, 1);
            Matrix<float> featuresM = new Matrix<float>(N, D);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < D; j++)
                    featuresM[i, j] = 100 * (float)r.NextDouble() - 50;

            EMParams pars = new EMParams();
            pars.CovMatType = Emgu.CV.ML.MlEnum.EM_COVARIAN_MATRIX_TYPE.COV_MAT_DIAGONAL;
            pars.Nclusters = G;
            pars.StartStep = Emgu.CV.ML.MlEnum.EM_INIT_STEP_TYPE.START_AUTO_STEP;
            pars.TermCrit = new MCvTermCriteria(100, 1.0e-6);

            Em.Train(featuresM, null, pars, labels);
            IntPtr Means = Em.Means.MCvMat.data;
            double x;
            unsafe
            {
                double* PTR = (double*)Means;
                x = *PTR;
            }
            Matrix<double>[] Covariance = Em.GetCovariances();
        }
        public void Kmean(Window _Window)
        {
            int Classes = 2;

            #region Get Random Centroids
            double[] centroids = new double[Classes];
            Random rand = new Random();
            for (int k = 0; k < Classes; k++)
            {
                int i = rand.Next(0, _Window.WinFrame.height);
                int j = rand.Next(0, _Window.WinFrame.width);
                centroids[k] = (_Window.WinFrame.redPixels[i, j] + _Window.WinFrame.greenPixels[i, j] + _Window.WinFrame.bluePixels[i, j]) / 3;
            }
            #endregion

            #region Centroids Convergence
            bool change = true;
            while (change)
            {
                double[] CentroidAssignedValues = new double[Classes];
                double[] CentroidAssignedFrequencies = new double[Classes];

                for (int i = 0; i < _Window.WinFrame.height; i++)
                {
                    for (int j = 0; j < _Window.WinFrame.width; j++)
                    {
                        double Difference = Math.Abs(((double)(_Window.WinFrame.redPixels[i, j] + _Window.WinFrame.greenPixels[i, j] + _Window.WinFrame.bluePixels[i, j]) / 3) - centroids[0]);
                        double Difference2 = Math.Abs(((double)(_Window.WinFrame.redPixels[i, j] + _Window.WinFrame.greenPixels[i, j] + _Window.WinFrame.bluePixels[i, j]) / 3) - centroids[1]);
                        if (Difference < Difference2)
                        {
                            CentroidAssignedFrequencies[0]++;
                            CentroidAssignedValues[0] += ((_Window.WinFrame.redPixels[i, j] + _Window.WinFrame.greenPixels[i, j] + _Window.WinFrame.bluePixels[i, j]) / 3);
                            _Window.AfterCalcPointClass[i, j] = 1;
                        }
                        else
                        {
                            CentroidAssignedFrequencies[1]++;
                            CentroidAssignedValues[1] += ((_Window.WinFrame.redPixels[i, j] + _Window.WinFrame.greenPixels[i, j] + _Window.WinFrame.bluePixels[i, j]) / 3);
                            _Window.AfterCalcPointClass[i, j] = 2;
                        }
                    }
                }
                change = false;
                for (int c = 0; c < Classes; c++)
                {
                    double temp = CentroidAssignedFrequencies[c] / (_Window.WinFrame.width * _Window.WinFrame.height);
                    if (Math.Abs(temp - centroids[c]) != 0)
                    {
                        centroids[c] = temp;
                        change = true;
                    }
                }
            }
            #endregion

            double MuF = centroids[0];
            double MuB = centroids[1];
        }
    }
}