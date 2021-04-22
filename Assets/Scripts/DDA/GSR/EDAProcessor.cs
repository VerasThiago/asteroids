using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

//NAO USAMOS MAIS. Era usado no código do Marcos para calcular o nível Tônico
public class EDAProcessor {

    //private int sampleRate = 4;
    private int arousalLevel = 1000;
    //private double defaultTimeWindow = 10;

    private double minArousalArea;
    private double maxArousalArea;
    private double minTonicAmplitude;
    private double maxTonicAmplitude;
    private double minMovingAverage;
    private double maxMovingAverage;

    public EDAProcessor()
    {
        Reset();
    }

    public void Reset()
    {
        minArousalArea = double.NaN;
        maxArousalArea = double.NaN;
        minTonicAmplitude = double.NaN;
        maxTonicAmplitude = double.NaN;
        minMovingAverage = double.NaN;
        maxMovingAverage = double.NaN;
    }

    // PHASIC

    /// <summary>
    /// Define signal values that will be included in calculations of EDA features.
    /// </summary>
    ///
    /// <param name="signalValueInCache"> List with all signal value store in the cache. </param>
    /// <param name="numberOfAffectedPoints"> Number of affected signal values. </param>
    ///
    /// <returns>
    /// List of signal values that will be included in calculations of EDA features.
    /// </returns>
    public List<EDASignal> AffectedCoordinatePoints(List<EDASignal> signalValueInCache, int numberOfAffectedPoints)
    {
        List<EDASignal> result = new List<EDASignal>();

        for (int i = (signalValueInCache.Count - numberOfAffectedPoints); i < signalValueInCache.Count - 1; i++)
        {
            result.Add(signalValueInCache[i]);
        }

        return result;
    }

    /// <summary>
    /// Calculate area of the signal. 
    /// </summary>
    ///
    /// <param name="coordinates"> List of inflection points. </param>
    /// <param name="numberOfAffectedPoints"> Number of signal values (depending on time window) that should participate in the calculation. </param>
    /// <param name="timeWindow"> Time window. </param>
    ///
    /// <returns>
    /// Area of the signal.
    /// </returns>
    private double GetArousalArea(List<EDASignal> coordinates, double timeWindow)
    {
        /*
        * For each one point P1 we take the next one P2 and
        * calculate the area of trapezoid or rectangular or two triangulars.
        * if P1.Y and P2.Y are in the same quadrant: S = (|P1.Y| + |P2.Y|)*(P2.X - P1.X)/2
        * else S = S1 + S2, where
        * S1 is the area of the triangular with peaks P1.Y, P1.X, Pm.X
        * S2 is the area of the triangular with peaks P2.Y, P2.X, Pm.X
        * Pm is the intercept point between the line (P1, P2) and y=0
        */
        double area = 0;

        for (int i = 0; i < (coordinates.Count - 1); i++)
        {

            if (i < (coordinates.Count - 2))
            {
                double x1 = coordinates[i].time;
                double y1 = coordinates[i].value;
                //double y1 = coordinates[i].HighPassValue;
                double x2 = coordinates[i + 1].time;
                double y2 = coordinates[i + 1].value;
                //double y2 = coordinates[i + 1].HighPassValue;

                if (y1 * y2 >= 0)
                {
                    area += (Math.Abs(y1) + Math.Abs(y2)) * (x2 - x1) / 2;
                }
                else if ((x2 - x1) != 0)
                {
                    // find x where y = 0
                    // y = a.x+b => X = -b/a
                    double a = (y2 - y1) / (x2 - x1);
                    double b = y1 - (a * x1);
                    double xIntercept = (-1) * b / a;
                    area += ((Math.Abs(y1)) * (xIntercept - x1) + (Math.Abs(y2)) * (x2 - xIntercept)) / 2;
                }
            }
        }

        return (area / timeWindow);
    }

    public int GetPhasicLevel(List<EDASignal> coordinates)
    {
        //List<EDASignal> highPassCoordinatesByTimeWindow = AffectedCoordinatePoints(coordinates, coordinates.Count);
        double SCRArousalArea = GetArousalArea(coordinates, 0.25f * coordinates.Count);/*highPassCoordinatesByTimeWindow, defaultTimeWindow);*/
        return GetPhasicLevel(SCRArousalArea); // 4Hz -> 0.25f
    }

    /// <summary>
    /// Define the phasic level depending on the phasic arousal area.
    /// </summary>
    /// 
    /// <param name="scrArousalArea">SCR arousal area.</param>
    /// 
    /// <returns>
    /// Phasic level (the level of arousal).
    /// </returns>
    private int GetPhasicLevel(double scrArousalArea)
    {
        if (double.IsNaN(minArousalArea) && double.IsNaN(maxArousalArea))
        {
            minArousalArea = scrArousalArea;
            maxArousalArea = scrArousalArea;
            return arousalLevel / 2;
        }
        if (scrArousalArea.CompareTo(minArousalArea) <= 0)
        {
            minArousalArea = scrArousalArea;
            return 1;
        }

        if (scrArousalArea.CompareTo(maxArousalArea) >= 0)
        {
            maxArousalArea = scrArousalArea;
            return arousalLevel;
        }

        double step = (arousalLevel != 0) ? (maxArousalArea - minArousalArea) / arousalLevel : 0.0;
        return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((scrArousalArea - minArousalArea) / step) : 0;
    }

    /// <summary>
    /// Define the extremum type of an signal value - whether it is a minimum or maximum.
    /// </summary>
    ///
    /// <param name="signalValueList"> List with all signal value. </param>
    /// <param name="i"> Index of the target inflection point from the list with inflection points. </param>
    ///
    /// <returns>
    /// The type extremum of the target inflection point.
    /// </returns>
    private static InflectionPoint.ExtremaType GetExtremaType(double? previousSignalValue, double signalValue, double? nextSignalValue)
    {
        if (!previousSignalValue.HasValue)
        {
            return (signalValue.CompareTo(nextSignalValue) >= 0) ? InflectionPoint.ExtremaType.Maximum : InflectionPoint.ExtremaType.Minimum;
        }
        else if (!nextSignalValue.HasValue)
        {
            return (signalValue.CompareTo(previousSignalValue) >= 0) ? InflectionPoint.ExtremaType.Maximum : InflectionPoint.ExtremaType.Minimum;
        }
        else if (signalValue.CompareTo(previousSignalValue) >= 0 && signalValue.CompareTo(nextSignalValue) >= 0)
        {
            return InflectionPoint.ExtremaType.Maximum;
        }
        else if (signalValue.CompareTo(previousSignalValue) <= 0 && signalValue.CompareTo(nextSignalValue) <= 0)
        {
            return InflectionPoint.ExtremaType.Minimum;
        }

        return InflectionPoint.ExtremaType.None;
    }

    /// <summary>
    /// Looking for inflection points in a list of signal values.
    /// If we have a sequence of more than two signal values with the same value (points are collinear)
    /// we take for inflection points only the first and last.
    /// </summary>
    ///
    /// <param name="signalCoordinatePoints"> List of signal values. </param>
    ///
    /// <returns>
    /// List of founded inflection points.
    /// </returns>
    public List<InflectionPoint> GetInflectionPoints(List<EDASignal> signalCoordinatePoints, string dataType)
    {
        List<InflectionPoint> inflectionPoints = new List<InflectionPoint>();
        int candidateInflectionPoint = -1;

        //add the first and the last signal value
        //comentei essas linhas pq nao to usando o high e o low value e add as de baixo
        //double firstInflectionPoints = (("default").Equals(dataType)) ? signalCoordinatePoints[0].value : (("highPass").Equals(dataType)) ? signalCoordinatePoints[0].HighPassValue : signalCoordinatePoints[0].LowPassValue;
        //double secondInflectionPoints = (("default").Equals(dataType)) ? signalCoordinatePoints[1].value : (("highPass").Equals(dataType)) ? signalCoordinatePoints[1].HighPassValue : signalCoordinatePoints[1].LowPassValue;
        double firstInflectionPoints = signalCoordinatePoints[0].value;
        double secondInflectionPoints = signalCoordinatePoints[1].value;
        inflectionPoints.Add(new InflectionPoint(signalCoordinatePoints[0], 0, GetExtremaType(null, firstInflectionPoints, secondInflectionPoints)));
        //comentei essas linhas pq nao to usando o high e o low value e add as de baixo
        //double penultimateInflectionPoint = (("default").Equals(dataType)) ? signalCoordinatePoints[signalCoordinatePoints.Count - 2].value : (("highPass").Equals(dataType)) ? signalCoordinatePoints[signalCoordinatePoints.Count - 2].HighPassValue : signalCoordinatePoints[signalCoordinatePoints.Count - 2].LowPassValue;
        //double lastInflectionPoint = (("default").Equals(dataType)) ? signalCoordinatePoints[signalCoordinatePoints.Count - 1].value : (("highPass").Equals(dataType)) ? signalCoordinatePoints[signalCoordinatePoints.Count - 1].HighPassValue : signalCoordinatePoints[signalCoordinatePoints.Count - 1].LowPassValue;
        double penultimateInflectionPoint = signalCoordinatePoints[signalCoordinatePoints.Count - 2].value;
        double lastInflectionPoint = signalCoordinatePoints[signalCoordinatePoints.Count - 1].value;
        inflectionPoints.Add(new InflectionPoint(signalCoordinatePoints[signalCoordinatePoints.Count - 1], signalCoordinatePoints.Count - 1, GetExtremaType(penultimateInflectionPoint, lastInflectionPoint, null)));

        double currentFindLastInflectionPoint = firstInflectionPoints;

        for (int i = 1; i < (signalCoordinatePoints.Count - 1); i++) {
            //comentei essas linhas pq nao to usando o high e o low value e add as de baixo
            //double currentPointY = (("default").Equals(dataType)) ? signalCoordinatePoints[i].value : (("highPass").Equals(dataType)) ? signalCoordinatePoints[i].HighPassValue : signalCoordinatePoints[i].LowPassValue;
            //double previousPointY = (("default").Equals(dataType)) ? signalCoordinatePoints[i - 1].value : (("highPass").Equals(dataType)) ? signalCoordinatePoints[i - 1].HighPassValue : signalCoordinatePoints[i - 1].LowPassValue;
            //double nextPointY = (("default").Equals(dataType)) ? signalCoordinatePoints[i + 1].value : (("highPass").Equals(dataType)) ? signalCoordinatePoints[i + 1].HighPassValue : signalCoordinatePoints[i + 1].LowPassValue;
            double currentPointY = signalCoordinatePoints[i].value;
            double previousPointY = signalCoordinatePoints[i - 1].value;
            double nextPointY = signalCoordinatePoints[i + 1].value;
            InflectionPoint.ExtremaType extremumType = GetExtremaType(previousPointY, currentPointY, nextPointY);

            if (!extremumType.Equals(InflectionPoint.ExtremaType.None))
            {
                if (currentPointY.CompareTo(currentFindLastInflectionPoint) != 0)
                {
                    if (i > 0 && inflectionPoints.Count > 0)
                    {
                        if ((candidateInflectionPoint + 1) == i &&
                            inflectionPoints[inflectionPoints.Count - 1].CoordinateY.Equals(currentFindLastInflectionPoint))
                        {
                            inflectionPoints.Add(new InflectionPoint(signalCoordinatePoints[candidateInflectionPoint], candidateInflectionPoint, extremumType));
                        }
                    }

                    inflectionPoints.Add(new InflectionPoint(signalCoordinatePoints[i], i, extremumType));
                }
                else
                {
                    candidateInflectionPoint = i;
                }

                currentFindLastInflectionPoint = currentPointY;
            }
            /*else
            {
                if (i > 0 && inflectionPoints.Count > 0)
                {
                    if ((candidateInflectionPoint + 1) == i && inflectionPoints[inflectionPoints.Count - 1].CoordinateY.Equals(signalCoordinatePoints[i - 1].SignalValue))
                    {
                        inflectionPoints.Add(new InflectionPoint(signalCoordinatePoints[candidateInflectionPoint], candidateInflectionPoint, extremumType));
                    }
                }
            }*/
        }

        return inflectionPoints;
    }

    // TONIC

    private static decimal GetTonicMean(List<double> allMaximums, double sumMaximums, double tonicMeanAmp)
    {
        if (allMaximums.Count == 1) return Convert.ToDecimal(tonicMeanAmp, CultureInfo.InvariantCulture);
        return (allMaximums != null && allMaximums.Count > 0) ? Convert.ToDecimal(sumMaximums / allMaximums.Count, CultureInfo.InvariantCulture) : 0;
    }

    /// <summary>
    /// Calculate standard deviation of a list of numbers 
    /// </summary>
    ///
    /// <param name="listOfNumbers"> List of numbers. </param>
    /// <param name="mean"> Mean of the list listOfNumbers. </param>
    ///
    /// <returns>
    /// Standard deviation.
    /// </returns>
    private decimal GetStandardDeviation(List<double> listOfNumbers, decimal mean)
    {
        double stdDeviation = 0;
        foreach (double currentNumber in listOfNumbers)
        {
            stdDeviation += Math.Pow((currentNumber - (double)mean), 2);
        }

        return (listOfNumbers.Count > 0) ? Convert.ToDecimal(Math.Sqrt(stdDeviation / listOfNumbers.Count), CultureInfo.InvariantCulture) : 0;
    }

    public int GetTonicLevel(List<EDASignal> coordinates)
    {
        //List<EDASignal> highPassCoordinatesByTimeWindow = AffectedCoordinatePoints(coordinates, coordinates.Count);
        List<InflectionPoint> inflectionPoints = GetInflectionPoints(coordinates, "default");

        // codigo de GetTonicStatisticsForPoints
        double tonicCoordinateXFirst = inflectionPoints[0].CoordinateX;
        double tonicCoordinateXLast = inflectionPoints[inflectionPoints.Count - 1].CoordinateX;
        double tonicCoordinateYFirst = inflectionPoints[0].CoordinateY;
        double tonicCoordinateYLast = inflectionPoints[inflectionPoints.Count - 1].CoordinateY;

        double MeanAmp = (tonicCoordinateYFirst + tonicCoordinateYLast) / 2;
        /*double Slope = (tonicCoordinateYLast - tonicCoordinateYFirst) / (tonicCoordinateXLast - tonicCoordinateXFirst);

        List<double> allMaximums = new List<double>();
        double minTonic = inflectionPoints[0].CoordinateY;
        double maxTonic = inflectionPoints[inflectionPoints.Count - 1].CoordinateY;

        double sumMaximums = 0;

        for (int i = 0; i < inflectionPoints.Count; i++)
        {
            if (inflectionPoints[i].ExtremumType.Equals(InflectionPoint.ExtremaType.Maximum))
            {
                double currentY = inflectionPoints[i].CoordinateY;
                minTonic = (minTonic.CompareTo(currentY) > 0 && !currentY.Equals(0.0)) ? currentY : minTonic;
                maxTonic = (maxTonic.CompareTo(currentY) < 0) ? currentY : maxTonic;

                double currentAmplitude = currentY;
                allMaximums.Add(currentAmplitude);
                sumMaximums += currentAmplitude;
            }
        }

        double MinAmp = minTonic;
        double MaxAmp = maxTonic;
        decimal mean = GetTonicMean(allMaximums, sumMaximums, MeanAmp);

        decimal StdDeviation = GetStandardDeviation(allMaximums, mean);

        double dif = MeanAmp - MinAmp;

        //print("MEAN " + mean + " MEAN_AMP " + MeanAmp + " DEV " + StdDeviation);
        //print("MIN " + MinAmp + " MAX " + MaxAmp + " DIF " + dif);
        print("DEV " + (StdDeviation * 100) + " DIF " + (dif * 100));*/

        return GetTonicLevel(MeanAmp); // 4Hz -> 0.25f
    }

    /// <summary>
    /// Define the SCL level depending on the average values of tonic amplitudes.
    /// </summary>
    /// 
    /// <param name="tonicAverageAmplitude">Average values of tonic amplitudes.</param>
    /// 
    /// <returns>
    /// Tonic level.
    /// </returns>
    private int GetTonicLevel(double tonicAverageAmplitude)
        {
            if (Double.IsNaN(minTonicAmplitude) && Double.IsNaN(maxTonicAmplitude))
            {
                minTonicAmplitude = tonicAverageAmplitude;
                maxTonicAmplitude = tonicAverageAmplitude;
                return arousalLevel / 2;
            }
            
            if (tonicAverageAmplitude.CompareTo(minTonicAmplitude) <= 0)
            {
                minTonicAmplitude = tonicAverageAmplitude;
                return 1;
            }

            if (tonicAverageAmplitude.CompareTo(maxTonicAmplitude) >= 0)
            {
                maxTonicAmplitude = tonicAverageAmplitude;
                return arousalLevel;
            }

            double step = (arousalLevel != 0) ? (maxTonicAmplitude - minTonicAmplitude) / arousalLevel : 0.0;
            return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((tonicAverageAmplitude - minTonicAmplitude) / step) : 0;
    }

    // GENERAL

    /// <summary>
    /// Calculate average of amplitudes of the GSR signal values that is an indicator for the general arousal. 
    /// </summary>
    ///
    /// <param name="coordinates"> List of GSR signal values. </param>
    /// <param name="numberOfAffectedPoints"> Number of signal values (depending on time window) that should participate in the calculation. </param>
    ///
    /// <returns>
    /// Average of GSR signal amplitudes.
    /// </returns>
    private double GetMovingAverage(List<EDASignal> coordinates, int numberOfAffectedPoints)
    {
        double movingAverage = 0;
        for (int i = (coordinates.Count - numberOfAffectedPoints); i < coordinates.Count; i++)
        {
            movingAverage += Math.Abs(coordinates[i].value);
        }

        return (movingAverage / numberOfAffectedPoints);
    }

    public int GetGeneralArousalLevel(List<EDASignal> coordinates)
    {
        double MovingAverage = GetMovingAverage(coordinates, coordinates.Count);
        return GetGeneralArousalLevel(MovingAverage);
    }

    /// <summary>
    /// Define the general arousal level depending on the moving average.
    /// </summary>
    /// 
    /// <param name="movingAverage">average of signal amplitudes (after median filter) for the last time window</param>
    /// 
    /// <returns>
    /// General level of arousal.
    /// </returns>
    private int GetGeneralArousalLevel(double movingAverage)
    {
        if (double.IsNaN(minMovingAverage) && double.IsNaN(maxMovingAverage))
        {
            minMovingAverage = 0.75 * movingAverage;
            maxMovingAverage = 1.25 * movingAverage;
        }

        if (movingAverage.CompareTo(minMovingAverage) <= 0)
        {
            minMovingAverage = movingAverage;
            return 1;
        }

        if (movingAverage.CompareTo(maxMovingAverage) >= 0)
        {
            maxMovingAverage = movingAverage;
            return arousalLevel;
        }

        double step = (arousalLevel != 0) ? (maxMovingAverage - minMovingAverage) / arousalLevel : 0.0;
        return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((movingAverage - minMovingAverage) / step) : 0;
    }
}
