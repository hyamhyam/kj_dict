// Copyright 2008 hyam <hhhyam@ybb.ne.jp>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
using System;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;


namespace HYAM.KJ_dict
{

    public class KJ_Util 
    {

        //------------------------------------------------------------
        // OS関係
     //   static public bool  Is2000orXP (){
     //       
     //       System.OperatingSystem os = System.Environment.OSVersion;
     //       if(os.Platform==PlatformID.Win32NT   &&
     //         os.Version.Major >= 5){
     //           return true;
     //       }
     //       
     //       return false;
     //   }

       
        //------------------------------------------------------------
        // Sound関係
        private static System.Media.SoundPlayer player = null;
        //
        //WAVファイルを再生する
        public static void PlaySound(string waveFile)
        {

            //再生されているときは止める
            if (player != null){
                StopSound();
            }

            //読み込む
            player = new System.Media.SoundPlayer(waveFile);
            //非同期再生する
            player.Play();

            //次のようにすると、ループ再生される
            //player.PlayLooping();

            //次のようにすると、最後まで再生し終えるまで待機する
            //player.PlaySync();
        }
        
        // streamを再生する(オーバーロード)
        public static void PlaySound(System.IO.Stream stream)
        {
            //再生されているときは止める
            if (player != null){
                StopSound();
            }

            //読み込む
            player = new System.Media.SoundPlayer(stream);
            //非同期再生する
            player.Play();
        }

        //再生されている音を止める
        public static void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }
        //------------------------------------------------------------

    }



} // end of namespace
