using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        // For the most part this is a rip-off from KOS
        [KSClass("MovingAverage")]
        public class MovingAverage {
            public List<double> Values { get; set; }

            [KSField] public double Mean { get; private set; }

            [KSField]
            public long ValueCount {
                get { return Values.Count; }
            }

            [KSField] public long SampleLimit { get; set; }

            public MovingAverage() {
                Reset();
                SampleLimit = 30;
            }

            [KSMethod]
            public void Reset() {
                Mean = 0;
                if (Values == null) Values = new List<double>();
                else Values.Clear();
            }

            [KSMethod]
            public double Update(double value) {
                if (double.IsInfinity(value) || double.IsNaN(value)) return value;

                Values.Add(value);
                while (Values.Count > SampleLimit) {
                    Values.RemoveAt(0);
                }

                //if (Values.Count > 5) Mean = Values.OrderBy(e => e).Skip(1).Take(Values.Count - 2).Average();
                //else Mean = Values.Average();
                //Mean = Values.Average();
                double sum = 0;
                double count = 0;
                double max = double.MinValue;
                double min = double.MaxValue;
                for (int i = 0; i < Values.Count; i++) {
                    double val = Values[i];
                    if (val > max) {
                        if (max != double.MinValue) {
                            sum += max;
                            count++;
                        }

                        max = val;
                    } else if (val < min) {
                        if (min != double.MaxValue) {
                            sum += min;
                            count++;
                        }

                        min = val;
                    } else {
                        sum += val;
                        count++;
                    }
                }

                if (count == 0) {
                    if (max != double.MinValue) {
                        sum += max;
                        count++;
                    }

                    if (min != double.MaxValue) {
                        sum += min;
                        count++;
                    }
                }

                Mean = sum / count;
                return Mean;
            }
        }
    }
}
