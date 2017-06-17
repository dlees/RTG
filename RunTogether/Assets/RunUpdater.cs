using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class RunUpdater : NetworkBehaviour
{

    public Text LabelText;
    public Text DistanceText;
    public Text LatAndLongText;

    [SyncVar]
    private double TotalDistance = 0.0;
    private double Distance_1s = 0.0;

    private double lat_1 = 360.0;
    private double long_1 = 360.0;

    [SyncVar]
    private double lat_2 = 360.0;
    [SyncVar]
    private double long_2 = 360.0;

    private char unit = 'K';

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("UpdateDistance", 0, 1);

        if (!isLocalPlayer)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1);
        }
        else
        {
            Input.location.Start();
        }
    }

    void UpdateDistance()
    {
        if (isLocalPlayer)
        {
            // If you want more data (like altitude) add it to the function here
            CmdUpdateLocation(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }
    }

    [Command]
    public void CmdUpdateLocation(double latitude, double longitude)
    {
        // Calculations for distance, velocity, etc should go here.

        //TotalDistanceFloat += latitude;

        lat_1 = latitude;
        long_1 = longitude;

        if (lat_2 == 360.0)
        {
            lat_2 = lat_1;
        }
        if (long_2 == 360.0)
        {
            long_2 = long_1;
        }

        Distance_1s = distance_fn(lat_1, long_1, lat_2, long_2, unit) * 1000; //in meters

        TotalDistance += Distance_1s;

        lat_2 = latitude;
        long_2 = longitude;

        /*transform.position = Quaternion.AngleAxis(longitude, -Vector3.up) * Quaternion.AngleAxis(latitude, -Vector3.right) * new Vector3(0, 0, 1);
        // FYI transform.position is where the text is on the screen. If you change it, it will move. You probably want anotherVARIABLE
        TotalDistance += transform.position;*/

    }

    void Update()
    {
        DistanceText.text = TotalDistance.ToString();
        LatAndLongText.text = lat_2.ToString("F4") + ", " + long_2.ToString("F4");

        if (!isLocalPlayer)
        {
            return;
        }


        /*var y = Input.GetAxis("Vertical") * Time.deltaTime * 50.0f;
        transform.Translate(0, y, 0);*/
    }

    public override void OnStartLocalPlayer()
    {
        LabelText.text = "You:";
    }

    private double distance_fn(double lat1, double lon1, double lat2, double lon2, char unit)
    {
        double theta = lon1 - lon2;
        double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
        dist = Math.Acos(dist);
        dist = rad2deg(dist);
        dist = dist * 60 * 1.1515;
        if (unit == 'K')
        {
            dist = dist * 1.609344;
        }
        else if (unit == 'N')
        {
            dist = dist * 0.8684;
        }
        return (dist);
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  This function converts decimal degrees to radians             :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private double deg2rad(double deg)
    {
        return (deg * Math.PI / 180.0);
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  This function converts radians to decimal degrees             :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private double rad2deg(double rad)
    {
        return (rad / Math.PI * 180.0);
    }
}
