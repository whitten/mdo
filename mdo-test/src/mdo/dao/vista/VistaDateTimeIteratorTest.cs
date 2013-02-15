#region CopyrightHeader
//
//  Copyright by Contributors
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//         http://www.apache.org/licenses/LICENSE-2.0.txt
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace gov.va.medora.mdo.dao.vista
{
    [TestFixture]
    public class VistaDateTimeIteratorTest
    {

        [Test]
        [Category("unit_only")]
        public void TestSetIteratorLengthFromDates()
        {
            VistaDateTimeIterator testIterator = new VistaDateTimeIterator("20080102.234556", "20080103");
            Assert.IsTrue(testIterator._iterLength == VistaDateTimeIterator.SECOND_ITERATION);
            Assert.AreEqual((int)VistaDateTimeIterator.PRECISION.SECOND, testIterator._precision);
        }

        [Test]
        [Category("unit_only")]
        public void TestSetIteratorLengthFromEndDate()
        {
            VistaDateTimeIterator testIterator = new VistaDateTimeIterator("20080102", "20080103.01");
            Assert.IsTrue(testIterator._iterLength == VistaDateTimeIterator.HOUR_ITERATION);
            Assert.AreEqual((int)VistaDateTimeIterator.PRECISION.HOUR, testIterator._precision);
        }

        [Test]
        [Category("unit_only")]
        public void TestSetIteratorLengthFromEndDateZero()
        {
            VistaDateTimeIterator testIterator = new VistaDateTimeIterator("20080102", "20080103.00");
            Assert.IsTrue(testIterator._iterLength == VistaDateTimeIterator.HOUR_ITERATION);
            Assert.AreEqual((int)VistaDateTimeIterator.PRECISION.HOUR, testIterator._precision);
        }

        /// <summary>
        /// BEWARE: A DateTime string that gets padded with 0s will cause you to iterate
        /// in seconds.
        /// </summary>
        [Test]
        [Category("unit_only")]
        public void TestSetIteratorLengthNormalizedNumber()
        {
            VistaDateTimeIterator testIterator = new VistaDateTimeIterator("20080102.000000", "20080103.0000");
            Assert.IsTrue(testIterator._iterLength == VistaDateTimeIterator.SECOND_ITERATION);
            Assert.AreEqual((int)VistaDateTimeIterator.PRECISION.SECOND, testIterator._precision);
        }

        /// <summary>Dummy check...</summary>
        [Test]
        [Category("unit_only")]
        public void TestPrecisionPositions()
        {
            string testDate = VistaTimestamp.fromDateTime(new DateTime(2008, 01, 23, 12, 34, 56, 789));
            Assert.AreEqual("308",testDate.Substring(0,(int)VistaDateTimeIterator.PRECISION.YEAR));
            Assert.AreEqual("30801", testDate.Substring(0,(int)VistaDateTimeIterator.PRECISION.MONTH));
            Assert.AreEqual("3080123", testDate.Substring(0,(int)VistaDateTimeIterator.PRECISION.DAY));
            Assert.AreEqual("3080123.12", testDate.Substring(0, (int)VistaDateTimeIterator.PRECISION.HOUR));
            Assert.AreEqual("3080123.1234", testDate.Substring(0, (int)VistaDateTimeIterator.PRECISION.MINUTE));
            Assert.AreEqual("3080123.123456", testDate.Substring(0, (int)VistaDateTimeIterator.PRECISION.SECOND));
        }

       public string CalculateMD5Hash(string input)
        {
          // step 1, calculate MD5 hash from input
          System.Security.Cryptography.MD5 md5
            = System.Security.Cryptography.MD5.Create();
          byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
          byte[] hash = md5.ComputeHash(inputBytes);
          // step 2, convert byte array to hex string
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < hash.Length; i++)
            {
            sb.Append(hash[i].ToString("X2"));
            }
          return sb.ToString();
        }

        /// <summary>
        /// Test to see that as we roll over days, that the iterator will do the right thing as well.
        /// </summary>
        [Test]
        [Category("unit_only")]
        public void TestHourRollOver()
        {
          //http://msdn.microsoft.com/en-us/library/system.string.gethashcode.aspx
          //   If two string objects are equal, the GetHashCode method returns identical values. However, there is not a unique hash code value for each unique string value. Different strings can return the same hash code. 
            VistaDateTimeIterator testIterator = new VistaDateTimeIterator(
                new DateTime(2008, 01, 01, 22, 0, 0)
                , new DateTime(2008, 01, 03)
                , new TimeSpan(1, 0, 0)
            );
            List<string> values = new List<string>();
            while (!testIterator.IsDone())
            {
                testIterator.SetIterEndDate(); // put at start of loop
                values.Add(testIterator.GetDdrListerPart());
                testIterator.AdvanceIterStartDate(); // put at end of loop
            }
            //Spot Check - Count
            Assert.AreEqual(26, values.Count);
            String[] strings = values.ToArray();
            //Spot Check - Validate Start Value
            Assert.IsTrue(strings[0].Equals("3080101.22"));
            //Spot Check - Validate End Value
            Assert.IsTrue(strings[25].Equals("3080102.23"));
            //Spot Check - Validate an Intermediate Value
            Assert.IsTrue(strings[14].Equals("3080102.12"));
            //The MD5 hash value is stable across platforms
            string hash = CalculateMD5Hash(string.Concat(values.ToArray()));
            Assert.AreEqual("830FAB9CC0EB3A1E3855B5DF0F560213", hash);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationDays()
        {
            int result = VistaDateTimeIterator.IterationDays("1.000000");
            Assert.AreEqual(1, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationDaysPoint()
        {
            int result = VistaDateTimeIterator.IterationDays(".000000");
            Assert.AreEqual(0, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationDays1Hour()
        {
            int result = VistaDateTimeIterator.IterationDays(".100000");
            Assert.AreEqual(0, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationDays1HourNoPoint()
        {
            int result = VistaDateTimeIterator.IterationDays("100000");
            Assert.AreEqual(0, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationHours()
        {
            int result = VistaDateTimeIterator.IterationHours("1.100000");
            Assert.AreEqual(10, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationHoursNoPoint()
        {
            int result = VistaDateTimeIterator.IterationHours("120000");
            Assert.AreEqual(12, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationHoursPoint()
        {
            int result = VistaDateTimeIterator.IterationHours(".345000");
            Assert.AreEqual(34, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationMinutes()
        {
            int result = VistaDateTimeIterator.IterationMinutes(".345000");
            Assert.AreEqual(50, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationSecond()
        {
            int result = VistaDateTimeIterator.IterationSeconds(".345036");
            Assert.AreEqual(36, result);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationTimeSpanFromString()
        {
            TimeSpan result = VistaDateTimeIterator.IterationTimeSpanFromString("1.010230");
            Assert.AreEqual(1, result.Days);
            Assert.AreEqual(1, result.Hours);
            Assert.AreEqual(2, result.Minutes);
            Assert.AreEqual(30, result.Seconds);
        }

        /// <summary>Values will roll over into next greater time range
        /// - e.g. seconds to minutes and hours to days
        /// </summary>
        [Test]
        [Category("unit_only")]
        public void TestIterationTimeSpanFromStringTooManySeconds()
        {
            TimeSpan result = VistaDateTimeIterator.IterationTimeSpanFromString("1.250290");
            Assert.AreEqual(2, result.Days);
            Assert.AreEqual(1, result.Hours);
            Assert.AreEqual(3, result.Minutes);
            Assert.AreEqual(30, result.Seconds);
        }

        /// <summary>Values will roll over into next greater time range
        /// - e.g. seconds to minutes and hours to days
        /// </summary>
        [Test]
        [Category("unit_only")]
        public void TestIterationTimeSpanFromStringOneDay()
        {
            TimeSpan result = VistaDateTimeIterator.IterationTimeSpanFromString("1.");
            Assert.AreEqual(1, result.Days);
            Assert.AreEqual(0, result.Hours);
            Assert.AreEqual(0, result.Minutes);
            Assert.AreEqual(0, result.Seconds);
        }

        /// <summary>
        /// This actually gives you a zero timespan, so the default of one day is
        /// used instead.
        /// </summary>
        [Test]
        [Category("unit_only")]
        public void TestIterationTimeSpanFromStringOne()
        {
            TimeSpan result = VistaDateTimeIterator.IterationTimeSpanFromString("1");
            Assert.AreEqual(1, result.Days);
        }

        /// <summary>
        /// One hour
        /// </summary>
        [Test]
        [Category("unit_only")]
        public void TestIterationTimeSpanFromStringDotOhOne()
        {
            TimeSpan result = VistaDateTimeIterator.IterationTimeSpanFromString(".01");
            Assert.AreEqual(1, result.Hours);
        }

        [Test]
        [Category("unit_only")]
        public void TestIterationTimeSpanFromStringOhOne()
        {
            TimeSpan result = VistaDateTimeIterator.IterationTimeSpanFromString("01");
            Assert.AreEqual(1, result.Hours);
        }
    }
}
