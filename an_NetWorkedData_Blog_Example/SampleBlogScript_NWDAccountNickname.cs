using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetWorkedData;
using System;

public class SampleBlogScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Set the nickname for current account and sync to get the unique nickname
    /// </summary>
    /// <param name="sNickname"></param>
#if NWD_ACCOUNT_IDENTITY
    public void SetCurrentNickname(string sNickname)
    {
        // get current data 
        NWDAccountNickname tNicknameData = NWDAccountNickname.CurrentData();
        // set nickname
        tNicknameData.Nickname = sNickname;
        // if data was changed, update it!
        tNicknameData.UpdateDataIfModified();
        // then sync async (or sync later)
        NWDData.Sync(new List<Type>() { typeof(NWDAccountNickname) });
    }
    /// <summary>
    /// Get the current account unique nickname
    /// </summary>
    /// <returns></returns>
    public string GetCurrentUniqueNickname()
    {
        string rUniqueNickname = string.Empty;
        // get current data 
        NWDAccountNickname tNicknameData = NWDAccountNickname.CurrentData();
        if (tNicknameData!=null)
        {
            // get nickname
            rUniqueNickname = tNicknameData.UniqueNickname;
        }
        return rUniqueNickname;
    }

    /// <summary>
    /// Set the nickname for current account,sync and return the unique nickname
    /// </summary>
    /// <param name="sNickname"></param>
    public string SetGetCurrentNickname(string sNickname)
    {
        // get current data 
        NWDAccountNickname tNicknameData = NWDAccountNickname.CurrentData();
        // set nickname
        tNicknameData.Nickname = sNickname;
        // if data was changed, update it!
        tNicknameData.UpdateDataIfModified();
        // then sync async (or sync later)
        NWDData.Sync(new List<Type>() { typeof(NWDAccountNickname) });
        return tNicknameData.UniqueNickname;
    }

    public bool SetNickname(string sNickname, string sForKey, int sMaxNickname)
    {
        bool rReturn = false;
        NWDAccountNickname tNicknameData = null;
        // get all datas and find the nickname for key
       NWDAccountNickname[] tNicknamesList = NWDBasisHelper.GetReachableDatas<NWDAccountNickname>();
        foreach (NWDAccountNickname tNicknameDataItem  in tNicknamesList)
        {
            if (tNicknameDataItem.InternalKey == sForKey)
            {
                tNicknameData = tNicknameDataItem;
                break;
            }
        }
        if (tNicknameData != null)
        {
            tNicknameData.Nickname = sNickname;
            // if data was changed, update it!
            tNicknameData.UpdateDataIfModified();
            NWDData.Sync(new List<Type>() { typeof(NWDAccountNickname) });
            rReturn = true;
        }
        else
        {
            if (tNicknamesList.Length < sMaxNickname)
            {
                tNicknameData = new NWDAccountNickname();
                tNicknameData.InternalKey = sForKey;
                tNicknameData.Nickname = sNickname;
                // if data was changed, update it!
                tNicknameData.UpdateDataIfModified();
                NWDData.Sync(new List<Type>() { typeof(NWDAccountNickname) });
                rReturn = true;
            }
        }
        return rReturn;
    }
#endif
}