/*
		Argentum Forever - Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
		gulfas@gmail.com
*/
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

public class XmlCharacterParser 
{

    public XmlCharacterParser(){
    }

    public void CreateFromXml(XmlDocument xml_doc,string selectnode){
        var nodes = xml_doc.SelectNodes(selectnode);
        Debug.Assert(nodes.Count>0);
        foreach (XmlNode nod in nodes)
        {

            mName   = nod["name"].InnerText;
            mUUID   = nod["uuid"].InnerText;
            Debug.Log("Name: " + mName);
            mMap = nod["position"]["map"].InnerText;
            Debug.Log("Position Map = " + mMap );
            string xstr = nod["position"]["x"].InnerText;
            string ystr = nod["position"]["y"].InnerText;
            Debug.Log("Position PosX = " + xstr );
            Debug.Log("Position PosY = " + ystr );
            float fx = float.Parse(xstr, CultureInfo.InvariantCulture.NumberFormat);
            float fy = float.Parse(ystr, CultureInfo.InvariantCulture.NumberFormat);
            mPos = Tuple.Create(fx,fy);
            Debug.Log("FPos " + mPos.ToString());
            //string skinColor = nod["skincolor"].InnerText;
            var skinColor = nod.SelectSingleNode("skincolor");
            Debug.Log("SKINCOLOR = " + skinColor + "*************************************");
            if (skinColor != null)
                mSkinColor = skinColor.InnerText;
            else
                mSkinColor = "5";
            //string size = nod["size"].InnerText;
        }
    }

    public Tuple<string,float,float> Position(){
        return Tuple.Create(mMap,mPos.Item1, mPos.Item2);
    }

    public string Name(){
        return mName;
    }
    public string UUID(){
        return mUUID;
    }
    public string SkinColor()
    {
        return mSkinColor;
    }
    private string mName;
    private string mUUID;
    private Tuple<float,float> mPos;
    private string mMap;
    private string mSkinColor;
    private string mSize;

}