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
            mPrefab   = nod["prefab"].InnerText;
            var isNpc = nod.SelectSingleNode("npc");
            if (isNpc != null)
                mIsNPC = isNpc.InnerText;
            mMap = nod["position"]["map"].InnerText;
            string xstr = nod["position"]["x"].InnerText;
            string ystr = nod["position"]["y"].InnerText;
            float fx = float.Parse(xstr, CultureInfo.InvariantCulture.NumberFormat);
            float fy = float.Parse(ystr, CultureInfo.InvariantCulture.NumberFormat);
            mPos = Tuple.Create(fx,fy);
            var skinColor = nod.SelectSingleNode("skincolor");
            if (skinColor != null)
                mSkinColor = skinColor.InnerText;
            else
                mSkinColor = "5";
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
    public string Prefab(){
        return mPrefab;
    }
    private string mName;
    private string mUUID;
    private string mIsNPC;
    private string mPrefab;
    private Tuple<float,float> mPos;
    private string mMap;
    private string mSkinColor;
    private string mSize;

}
