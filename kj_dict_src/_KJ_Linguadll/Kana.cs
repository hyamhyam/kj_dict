// Copyright 2004 hyam <hhhyam@ybb.ne.jp>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.

using System;

namespace HYAM.Lingua {
//-----------------------------------------------------------------------------

    

public class Kana {
    

    //------------------------------------------------------------------------
    // instance 
    public string    text;      // base text
    public string [] kRomanArray;    // romanaized char by array
    public string    hangul;     
    
    //------------------------------------------------------------------------
    // constructor
    public Kana(string text ) {
        this.text   = text;

        this.kRomanArray   = Kana2kRomanArray(this.text);
        this.hangul        = kRomanArray2Hangul(kRomanArray);
        
        
    }

    //------------------------------------------------------------------------
    // Korean Roman ---> Hnagul          (ex.   A,I,DOL --->  아이돌 )
    static private string  kRomanArray2Hangul( string [] kRomanArray ) {
        string hangul="";
        
        for ( int i=0 ; i < kRomanArray.Length ;  i++  ){
            hangul += Hangul.kRoman2Hangul(kRomanArray[i]);
        } // end of for
        
        return hangul;
    }
    


    //------------------------------------------------------------------------
    //  static public method 
    //------------------------------------------------------------------------
    //  Katakana --->  Hangul   (ex. アイドル ---> 아이돌)
    static public string  Kana2Hangul( string  kana ) {
        string [] kRomanArray   = Kana2kRomanArray(kana);
        string    hangul        = kRomanArray2Hangul(kRomanArray);
        return hangul ;
    }
    //  Hiragana --->  Hangul   (ex. あいどる ---> 아이돌)
    static public string  Hira2Hangul( string  hira ) {
        string kana = ConvTable.H2K(hira); // ひらがな→カタカナ
        return Kana2Hangul(kana);
    }
    
    
    //------------------------------------------------------------------------
    //  pritvate method 
    //------------------------------------------------------------------------
    //------------------------------------------------------------------------
    // Katakana ---> Korean Roman array    (ex.  アカネ --->  A,KA,NE  )
    static private string []  Kana2kRomanArray( string  kana ) {
        string [] tempArray; 
        tempArray  = new string [kana.Length] ;
        
        string kRoman="";
        int kcnt=0;
        for (int i=0; i < kana.Length ;  i++ )
        {
            if(kana[i]=='ー'){
                continue;  // 長音は(とりあえず)変換しない
            }
            
            kRoman += kana[i];
            if(i==(kana.Length-1)  ||  !isSmall(kana[i+1]) ){
                tempArray[kcnt] = kRoman;
                kcnt++;
                kRoman="";
            }
        }
        
        string [] kRomanArray  = new string [kcnt] ;
        for(int i=0 ; i < kcnt ; i++  ){
            kRomanArray[i] = ConvTable.K2R( tempArray[i] );
        }
        return kRomanArray;
    }



    //------------------------------------------------------------------------
    static private bool isSmall( char kana ) {
        if( kana=='ャ' || kana=='ュ' || kana=='ョ' || 
            kana=='ァ' || kana=='ィ' || kana=='ゥ' || kana=='ェ' || kana=='ォ' ||
            kana=='ン'  
          ){
            return true;
        }
        return false;
    }

    
} // end of class Kana



//-----------------------------------------------------------------------------
} // end of namespace  HYAM.Lingua

