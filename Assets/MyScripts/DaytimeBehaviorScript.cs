using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


// Adapted & Translated from mourner/suncalc: https://github.com/mourner/suncalc/blob/master/suncalc.js

public class DaytimeBehaviorScript : MonoBehaviour
{
    public GameObject target;

    public Slider slider;

    private DateTime date;
    private double currentAzimuth;
    private double currentAltitude;

    private double lat = 41.870275;
    private double lng = -87.650242;
    private double rad = Math.PI / 180;
    private double deg = 180 / Math.PI;

    private int dayMs = 1000 * 60 * 60 * 24;
    private int J1970 = 2440588;
    private int J2000 = 2451545;

    private double e = Math.PI / 180 * 23.4397; // obliquity of the Earth
    private double UnixEpoch;

    // Start is called before the first frame update
    void Start()
    {
        float hour = slider.value;
        var current = DateTime.Now;

        this.Date = current.AddHours(hour - current.Hour);
        Debug.Log(this.date.TimeOfDay.ToString());

        Debug.Log("Azimuth " + this.currentAzimuth * 180 / Math.PI);
        Debug.Log("Altitude " + this.currentAltitude * 180 / Math.PI);
    }

    // Update is called once per frame
    void Update()
    {
        //this.Date = this.Date.AddMinutes(1);
    }

    public void UpdateHour()
    {
        var current = date;
        float hour = slider.value;

        this.Date = current.AddHours(hour - current.Hour);

        //lightPrefab.transform.GetChild(1).GetComponent<Light>().intensity = hour < 7 || hour > 17 ? 0.8f : 0;
    }

    private DateTime Date
    {
        get {
            return date;
        }
        set {
            date = value;

            // (Azimuth, Altitude)
            var position = getPosition(date);

            currentAzimuth = position.Item1;
            currentAltitude = position.Item2;

            var R = 50;
            float X = R * (float)(Math.Cos(currentAltitude) * Math.Cos(currentAzimuth));
            float Z = R * (float)(Math.Cos(currentAltitude) * Math.Sin(currentAzimuth));
            float Y = R * (float)(Math.Sin(currentAltitude));

            gameObject.GetComponent<Light>().intensity = 0.1f + (
                currentAltitude < 0 ? 0 : 2 * (float)(currentAltitude / (Math.PI / 2))
            );
            gameObject.transform.position = new Vector3(X, Y, -Z) + target.transform.position;
            gameObject.transform.LookAt(target.transform.position);
        }
    }

    private Tuple<double, double> getPosition(DateTime date)
    {
        double lw = rad * -this.lng;
        double phi = rad * this.lat;
        double d = toDays(date);

        // (declination, ascension)
        Tuple<double, double> c = sunCoords(d);
        double H = siderealTime(d, lw) - c.Item2;

        return new Tuple<double, double>(azimuth(H, phi, c.Item1) + Math.PI / 2, altitude(H, phi, c.Item1));
    }

    private double toJulian(DateTime date)
    {
        var offset = date.ToUniversalTime().Subtract(
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds;

        return (offset / dayMs) - 0.5 + J1970;
    }

    private double toDays(DateTime date)
    {
        return toJulian(date) - J2000;
    }

    private Tuple<double, double> sunCoords(double d)
    {
        double M = solarMeanAnomaly(d);
        double L = elipticLongitude(M);

        return new Tuple<double, double>(declination(L, 0), rightAscension(L, 0));
    }

    private double solarMeanAnomaly(double d)
    {
        return rad * (357.5291 + 0.98560028 * d);
    }

    private double elipticLongitude(double M)
    {
        double C = rad * (1.9148 * Math.Sin(M) + 0.02 * Math.Sin(2 * M) + 0.0003 * Math.Sin(3 * M)); // center
        double P = rad * 102.9372; // perihelion of the Earth

        return M + C + P + Math.PI;
    }

    private double declination(double l, double b)
    {
        // asin(sin(b) * cos(e) + cos(b) * sin(e) * sin(l))
        return Math.Asin(Math.Sin(b) * Math.Cos(e) + Math.Cos(b) * Math.Sin(e) * Math.Sin(l));
    }

    private double rightAscension(double l, double b)
    {
        // atan(sin(l) * cos(e) - tan(b) * sin(e), cos(l))
        return Math.Atan2(Math.Sin(l) * Math.Cos(e) - Math.Tan(b) * Math.Sin(e), Math.Cos(l));
    }

    private double siderealTime(double d, double lw)
    {
        return rad * (280.16 + 360.9856235 * d) - lw;
    }

    private double azimuth(double H, double phi, double dec)
    {
        // atan(sin(H), cos(H) * sin(phi) - tan(dec) * cos(phi))
        return Math.Atan2(Math.Sin(H), Math.Cos(H) * Math.Sin(phi) - Math.Tan(dec) * Math.Cos(phi));
    }

    private double altitude(double H, double phi, double dec)
    {
        // asin(sin(phi) * sin(dec) + cos(phi) * cos(dec) * cos(H))
        return Math.Asin(Math.Sin(phi) * Math.Sin(dec) + Math.Cos(phi) * Math.Cos(dec) * Math.Cos(H));
    }
}