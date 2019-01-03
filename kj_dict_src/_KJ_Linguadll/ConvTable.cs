// Copyright 2004 hyam <hhhyam@ybb.ne.jp>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.

using System;
using System.Collections;

namespace HYAM.Lingua {
//-----------------------------------------------------------------------------

public class ConvTable {
    static Hashtable roma2kana;
    static Hashtable kana2roma;
    static Hashtable hira2kana;
    
    //--------------------------------------------------------------------
    // 静的コンストラクタ   
    static ConvTable(){
        
        //-----------------------------------------
        // Table 1
        roma2kana = new Hashtable();

        roma2kana.Add("A",  "ア") ;
        roma2kana.Add("I",  "イ") ;
        roma2kana.Add("U",  "ウ") ;
        roma2kana.Add("E",  "エ") ;
        roma2kana.Add("O",  "オ") ;

        //------------------------------------
        roma2kana.Add("YA",  "ヤ") ;
        roma2kana.Add("YI",  "ウィ") ;
        roma2kana.Add("YU",  "ユ") ;
        roma2kana.Add("YO",  "ヨ") ;
        roma2kana.Add("YE",  "エ") ;

        roma2kana.Add("WA",  "ワ") ;
        roma2kana.Add("WI",  "ウィ") ;
        roma2kana.Add("WO",  "ウォ") ;
        roma2kana.Add("WE",  "ウェ") ;

        roma2kana.Add("EO",  "ヱ") ;

        roma2kana.Add("N",  "ン") ;

        //------------------------------------
        roma2kana.Add("KA",  "カ") ;
        roma2kana.Add("KI",  "キ") ;
        roma2kana.Add("KU",  "ク") ;
        roma2kana.Add("KE",  "ケ") ;
        roma2kana.Add("KO",  "コ") ;
         
        roma2kana.Add("KYA",  "キャ") ;
        roma2kana.Add("KYU",  "キュ") ;
        roma2kana.Add("KYO",  "キョ") ;
         
        roma2kana.Add("KWA",  "クァ") ;
        roma2kana.Add("KWU",  "クゥ") ;
        roma2kana.Add("KWE",  "クェ") ;
        roma2kana.Add("KWO",  "クォ") ;

        //------------------------------------
        roma2kana.Add("GA",  "ガ") ;
        roma2kana.Add("GI",  "ギ") ;
        roma2kana.Add("GU",  "グ") ;
        roma2kana.Add("GE",  "ゲ") ;
        roma2kana.Add("GO",  "ゴ") ;
         
        roma2kana.Add("GYA",  "ギャ") ;
        roma2kana.Add("GYU",  "ギュ") ;
        roma2kana.Add("GYO",  "ギョ") ;

        roma2kana.Add("GWA",  "グァ") ;
        roma2kana.Add("GWU",  "グゥ") ;
        roma2kana.Add("GWE",  "グェ") ;
        roma2kana.Add("GWO",  "グォ") ;
          
        //------------------------------------
        roma2kana.Add("SA",  "サ") ;
        roma2kana.Add("SI",  "シ") ;
        roma2kana.Add("SU",  "ス") ;
        roma2kana.Add("SE",  "セ") ;
        roma2kana.Add("SO",  "ソ") ;

        roma2kana.Add("SYA",  "シャ") ;
        roma2kana.Add("SYU",  "シュ") ;
        roma2kana.Add("SYO",  "ショ") ;
        
        roma2kana.Add("SWI",  "スィ") ;
         
        //------------------------------------
        roma2kana.Add("JA",  "ジャ") ;
        roma2kana.Add("JI",  "ジ") ;
        roma2kana.Add("JU",  "ジュ") ;
        roma2kana.Add("JE",  "ジェ") ;
        roma2kana.Add("JO",  "ジョ") ;

        roma2kana.Add("JYA",  "ジャ") ;
        roma2kana.Add("JYU",  "ジュ") ;
        roma2kana.Add("JYO",  "ジョ") ;
         
        //------------------------------------
        roma2kana.Add("TA",  "タ") ;
        roma2kana.Add("TI",  "ティ") ;
        roma2kana.Add("TU",  "ツ") ;
        roma2kana.Add("TE",  "テ") ;
        roma2kana.Add("TO",  "ト") ;

        roma2kana.Add("DWE",  "デェ") ;
        roma2kana.Add("DWU",  "ドゥ") ;
        roma2kana.Add("DWO",  "ドォ") ;
        roma2kana.Add("DA",  "ダ") ;
        roma2kana.Add("DI",  "ヂ") ;
        roma2kana.Add("DU",  "ドゥ") ;

        roma2kana.Add("DE",  "デ") ;
        roma2kana.Add("DO",  "ド") ;

        roma2kana.Add("DYA",  "ディャ") ;
        roma2kana.Add("DYI",  "ディ") ;
        roma2kana.Add("DYU",  "デュ") ;
        roma2kana.Add("DYE",  "デェ") ;
        roma2kana.Add("DYO",  "ディョ") ;

        roma2kana.Add("CYA",  "チャ") ;
        roma2kana.Add("CYU",  "チュ") ;
        roma2kana.Add("CYO",  "チョ") ;

        roma2kana.Add("CWA",  "チァ") ;
        roma2kana.Add("CWU",  "チュ") ;
        roma2kana.Add("CWE",  "チェ") ;
        roma2kana.Add("CWO",  "チョ") ;
          
        roma2kana.Add("CA",  "チャ") ;
        roma2kana.Add("CI",  "チ" ) ; 
        roma2kana.Add("CU",  "チュ") ;
        roma2kana.Add("CE",  "チェ") ;
        roma2kana.Add("CO",  "チョ") ;
         
        //------------------------------------
        roma2kana.Add("NA",  "ナ") ;
        roma2kana.Add("NI",  "ニ") ;
        roma2kana.Add("NU",  "ヌ") ;
        roma2kana.Add("NE",  "ネ") ;
        roma2kana.Add("NO",  "ノ") ;

        roma2kana.Add("NYA",  "ニャ") ;
        roma2kana.Add("NYU",  "ニュ") ;
        roma2kana.Add("NYO",  "ニョ") ;

        //------------------------------------
        roma2kana.Add("HA",  "ハ") ;
        roma2kana.Add("HI",  "ヒ") ;
        roma2kana.Add("HU",  "フ") ;
        roma2kana.Add("HE",  "ヘ") ;
        roma2kana.Add("HO",  "ホ") ;

        roma2kana.Add("HYA",  "ヒャ") ;
        roma2kana.Add("HYI",  "フィ") ;
        roma2kana.Add("HYU",  "ヒュ") ;
        roma2kana.Add("HYE",  "フェ") ;
        roma2kana.Add("HYO",  "ヒョ") ;
         
        roma2kana.Add("HWA",  "ファ") ;
        roma2kana.Add("HWI",  "フィ") ;
        roma2kana.Add("HWU",  "ヒゥ") ;
        roma2kana.Add("HWE",  "フェ") ;
        roma2kana.Add("HWO",  "フォ") ;
         
        //------------------------------------
        roma2kana.Add("BA",  "バ") ;
        roma2kana.Add("BI",  "ビ") ;
        roma2kana.Add("BU",  "ブ") ;
        roma2kana.Add("BE",  "ベ") ;
        roma2kana.Add("BO",  "ボ") ;
         
        roma2kana.Add("BYA",  "ビャ") ;
        roma2kana.Add("BYU",  "ビュ") ;
        roma2kana.Add("BYE",  "ビェ") ;
        roma2kana.Add("BYO",  "ビョ") ;

        roma2kana.Add("BWA",  "ボァ") ;
        roma2kana.Add("BWU",  "ビュ") ;
        roma2kana.Add("BWE",  "ボェ") ;
        roma2kana.Add("BWO",  "ビョ") ;
          
        //------------------------------------
        roma2kana.Add("PA",  "パ") ;
        roma2kana.Add("PI",  "ピ") ;
        roma2kana.Add("PU",  "プ") ;
        roma2kana.Add("PE",  "ペ") ;
        roma2kana.Add("PO",  "ポ") ;
        
//        roma2kana.Add("PPA",  "パッ") ;
//        roma2kana.Add("PPI",  "ピッ") ;
//        roma2kana.Add("PPU",  "プッ") ;
//        roma2kana.Add("PPE",  "ペッ") ;
//        roma2kana.Add("PPO",  "ポッ") ;

        roma2kana.Add("PYA",  "ピャ") ;
        roma2kana.Add("PYU",  "ピュ") ;
        roma2kana.Add("PYE",  "ピェ") ;
        roma2kana.Add("PYO",  "ピョ") ;
         
        roma2kana.Add("PWA",  "ポァ") ;
        roma2kana.Add("PWU",  "ピュ") ;
        roma2kana.Add("PWE",  "ポェ") ;
        roma2kana.Add("PWO",  "ピョ") ;
          
         
        //------------------------------------
        roma2kana.Add("MA",  "マ") ;
        roma2kana.Add("MI",  "ミ") ;
        roma2kana.Add("MU",  "ム") ;
        roma2kana.Add("ME",  "メ") ;
        roma2kana.Add("MO",  "モ") ;

        roma2kana.Add("MYA",  "ミャ") ;
        roma2kana.Add("MYU",  "ミュ") ;
        roma2kana.Add("MYE",  "ミェ") ;
        roma2kana.Add("MYO",  "ミョ") ;
         
        //------------------------------------
        roma2kana.Add("RA",  "ラ") ;
        roma2kana.Add("RI",  "リ") ;
        roma2kana.Add("RU",  "ル") ;
        roma2kana.Add("RE",  "レ") ;
        roma2kana.Add("RO",  "ロ") ;
         
        roma2kana.Add("RYA",  "リャ") ;
        roma2kana.Add("RYU",  "リュ") ;
        roma2kana.Add("RYE",  "リェ") ;
        roma2kana.Add("RYO",  "リョ") ;

        roma2kana.Add("RWA",  "リァ") ;
        roma2kana.Add("RWU",  "リュ") ;
        roma2kana.Add("RWE",  "リェ") ;
        roma2kana.Add("RWO",  "リョ") ;

        //---------------------------------------------------------------
        // Table 2
        kana2roma = new Hashtable();

        kana2roma.Add("ア",  "A") ;
        kana2roma.Add("イ",  "I") ;
        kana2roma.Add("ウ",  "U") ;
        kana2roma.Add("エ",  "E") ;
        kana2roma.Add("オ",  "O") ;

        kana2roma.Add("ウィ",  "YI") ;
        kana2roma.Add("ウェ",  "WAE") ;  // 왜
        kana2roma.Add("ウォ",  "WO") ;

        //-----------------------------------
        kana2roma.Add("カ",  "KA") ;
        kana2roma.Add("キ",  "KI") ;
        kana2roma.Add("ク",  "KU") ;
        kana2roma.Add("ケ",  "KE") ;
        kana2roma.Add("コ",  "KO") ;
        
        kana2roma.Add("ガ",  "GA") ;
        kana2roma.Add("ギ",  "GI") ;
        kana2roma.Add("グ",  "GU") ;
        kana2roma.Add("ゲ",  "GE") ;
        kana2roma.Add("ゴ",  "GO") ;

        kana2roma.Add("キャ",  "KYA") ;
        kana2roma.Add("キュ",  "KYU") ;
        kana2roma.Add("キョ",  "KYO") ;
 
        kana2roma.Add("ギャ",  "GYA") ;
        kana2roma.Add("ギュ",  "GYU") ;
        kana2roma.Add("ギョ",  "GYO") ;

        kana2roma.Add("クァ",  "KWA") ;
        kana2roma.Add("クゥ",  "KWU") ;
        kana2roma.Add("クェ",  "KWE") ;
        kana2roma.Add("クォ",  "KWO") ;
        
        kana2roma.Add("グァ",  "GWA") ;
        kana2roma.Add("グゥ",  "GWU") ;
        kana2roma.Add("グェ",  "GWE") ;
        kana2roma.Add("グォ",  "GWO") ;
 
        //-----------------------------------
        kana2roma.Add("サ",  "SA") ;
        kana2roma.Add("シ",  "SI") ;
        kana2roma.Add("ス",  "SU") ;
        kana2roma.Add("セ",  "SE") ;
        kana2roma.Add("ソ",  "SO") ;
        
        kana2roma.Add("ザ",  "JA") ;
        kana2roma.Add("ジ",  "JI") ;
        kana2roma.Add("ズ",  "JU") ;
        kana2roma.Add("ゼ",  "JE") ;
        kana2roma.Add("ゾ",  "JO") ;
        
        kana2roma.Add("シャ",  "SYA") ;
        kana2roma.Add("シュ",  "SYU") ;
        kana2roma.Add("ショ",  "SYO") ;
        
        kana2roma.Add("ジャ",  "JA") ;
        kana2roma.Add("ジュ",  "JU") ;
        kana2roma.Add("ジェ",  "JE") ;
        kana2roma.Add("ジョ",  "JO") ;
 
        //-----------------------------------
        kana2roma.Add("タ",  "TA") ;
        kana2roma.Add("チ",  "CI") ;
        kana2roma.Add("ツ",  "TU") ;
        kana2roma.Add("テ",  "TE") ;
        kana2roma.Add("ト",  "TO") ;

        kana2roma.Add("ティ",  "TI") ;
        kana2roma.Add("チャ",  "CA") ;
        kana2roma.Add("チュ",  "CU") ;
        kana2roma.Add("チェ",  "CE") ;
        kana2roma.Add("チョ",  "CO") ;
 
        kana2roma.Add("ヂャ",  "JYA") ;
        kana2roma.Add("ヂュ",  "JYU") ;
        kana2roma.Add("ヂョ",  "JYO") ;
        
        kana2roma.Add("ダ",  "DA") ;
        kana2roma.Add("ヂ",  "DI") ;
        kana2roma.Add("ヅ",  "DU") ;
        kana2roma.Add("デ",  "DE") ;
        kana2roma.Add("ド",  "DO") ;

        kana2roma.Add("デェ",  "DWE") ;
        kana2roma.Add("ドゥ",  "DU") ;
        kana2roma.Add("ドェ",  "DWE") ;
        kana2roma.Add("ドォ",  "DWO") ;

        //-----------------------------------
        kana2roma.Add("ナ",  "NA") ;
        kana2roma.Add("ニ",  "NI") ;
        kana2roma.Add("ヌ",  "NU") ;
        kana2roma.Add("ネ",  "NE") ;
        kana2roma.Add("ノ",  "NO") ;
 
        kana2roma.Add("ニャ",  "NYA") ;
        kana2roma.Add("ニュ",  "NYU") ;
        kana2roma.Add("ニョ",  "NYO") ;
 
        //-----------------------------------
        kana2roma.Add("ハ",  "HA") ;
        kana2roma.Add("ヒ",  "HI") ;
        kana2roma.Add("フ",  "HU") ;
        kana2roma.Add("ヘ",  "HE") ;
        kana2roma.Add("ホ",  "HO") ;
        
        kana2roma.Add("バ",  "BA") ;
        kana2roma.Add("ビ",  "BI") ;
        kana2roma.Add("ブ",  "BU") ;
        kana2roma.Add("ベ",  "BE") ;
        kana2roma.Add("ボ",  "BO") ;
        
        kana2roma.Add("パ",  "PA") ;
        kana2roma.Add("ピ",  "PI") ;
        kana2roma.Add("プ",  "PU") ;
        kana2roma.Add("ペ",  "PE") ;
        kana2roma.Add("ポ",  "PO") ;
        
        kana2roma.Add("ヒャ",  "HYA") ;
        kana2roma.Add("ヒュ",  "HYU") ;
        kana2roma.Add("ヒョ",  "HYO") ;
        kana2roma.Add("ヒゥ",  "HWU") ;

        kana2roma.Add("ファ",  "HWA") ;
        kana2roma.Add("フィ",  "HYI") ;
        kana2roma.Add("フェ",  "HYE") ;
        kana2roma.Add("フォ",  "HWO") ;

        kana2roma.Add("ビャ",  "BYA") ;
        kana2roma.Add("ビュ",  "BYU") ;
        kana2roma.Add("ビェ",  "BYE") ;
        kana2roma.Add("ビョ",  "BYO") ;

        kana2roma.Add("ピャ",  "PYA") ;
        kana2roma.Add("ピュ",  "PYU") ;
        kana2roma.Add("ピェ",  "PYE") ;
        kana2roma.Add("ピョ",  "PYO") ;
 
        //-----------------------------------
        kana2roma.Add("マ",  "MA") ;
        kana2roma.Add("ミ",  "MI") ;
        kana2roma.Add("ム",  "MU") ;
        kana2roma.Add("メ",  "ME") ;
        kana2roma.Add("モ",  "MO") ;
 
        kana2roma.Add("ミャ",  "MYA") ;
        kana2roma.Add("ミュ",  "MYU") ;
        kana2roma.Add("ミェ",  "MYE") ;
        kana2roma.Add("ミョ",  "MYO") ;
 
        //-----------------------------------
        kana2roma.Add("ラ",  "RA") ;
        kana2roma.Add("リ",  "RI") ;
        kana2roma.Add("ル",  "RU") ;
        kana2roma.Add("レ",  "RE") ;
        kana2roma.Add("ロ",  "RO") ;

        kana2roma.Add("リャ",  "RYA") ;
        kana2roma.Add("リュ",  "RYU") ;
        kana2roma.Add("リェ",  "RYE") ;
        kana2roma.Add("リョ",  "RYO") ;

        //-----------------------------------
        kana2roma.Add("ヤ",  "YA") ;
        kana2roma.Add("ユ",  "YU") ;
        kana2roma.Add("ヨ",  "YO") ;

        //-----------------------------------
        kana2roma.Add("ヲ",  "WO") ;
        kana2roma.Add("ヱ",  "EO") ;
        kana2roma.Add("ワ",  "WA") ;
        kana2roma.Add("ン",  "N") ;

        //-----------------------------------------
        // Table 3
        hira2kana = new Hashtable();

        hira2kana.Add("あ",  "ア") ;
        hira2kana.Add("い",  "イ") ;
        hira2kana.Add("う",  "ウ") ;
        hira2kana.Add("え",  "エ") ;
        hira2kana.Add("お",  "オ") ;

        hira2kana.Add("ぁ",  "ァ") ;
        hira2kana.Add("ぃ",  "ィ") ;
        hira2kana.Add("ぅ",  "ゥ") ;
        hira2kana.Add("ぇ",  "ェ") ;
        hira2kana.Add("ぉ",  "ォ") ;

        //------------------------------------
        hira2kana.Add("か",  "カ") ;
        hira2kana.Add("き",  "キ") ;
        hira2kana.Add("く",  "ク") ;
        hira2kana.Add("け",  "ケ") ;
        hira2kana.Add("こ",  "コ") ;
         
        hira2kana.Add("が",  "ガ") ;
        hira2kana.Add("ぎ",  "ギ") ;
        hira2kana.Add("ぐ",  "グ") ;
        hira2kana.Add("げ",  "ゲ") ;
        hira2kana.Add("ご",  "ゴ") ;
          
        //------------------------------------
        hira2kana.Add("さ",  "サ") ;
        hira2kana.Add("し",  "シ") ;
        hira2kana.Add("す",  "ス") ;
        hira2kana.Add("せ",  "セ") ;
        hira2kana.Add("そ",  "ソ") ;

        hira2kana.Add("ざ",  "ザ") ;
        hira2kana.Add("じ",  "ジ") ;
        hira2kana.Add("ず",  "ズ") ;
        hira2kana.Add("ぜ",  "ゼ") ;
        hira2kana.Add("ぞ",  "ゾ") ;
         
        //------------------------------------
        hira2kana.Add("た",  "タ") ;
        hira2kana.Add("ち",  "チ") ;
        hira2kana.Add("つ",  "ツ") ;
        hira2kana.Add("て",  "テ") ;
        hira2kana.Add("と",  "ト") ;

        hira2kana.Add("だ",  "ダ") ;
        hira2kana.Add("ぢ",  "ヂ") ;
        hira2kana.Add("づ",  "ヅ") ;
        hira2kana.Add("で",  "デ") ;
        hira2kana.Add("ど",  "ド") ;
         
        //------------------------------------
        hira2kana.Add("な",  "ナ") ;
        hira2kana.Add("に",  "ニ") ;
        hira2kana.Add("ぬ",  "ヌ") ;
        hira2kana.Add("ね",  "ネ") ;
        hira2kana.Add("の",  "ノ") ;

        //------------------------------------
        hira2kana.Add("は",  "ハ") ;
        hira2kana.Add("ひ",  "ヒ") ;
        hira2kana.Add("ふ",  "フ") ;
        hira2kana.Add("へ",  "ヘ") ;
        hira2kana.Add("ほ",  "ホ") ;
         
        hira2kana.Add("ば",  "バ") ;
        hira2kana.Add("び",  "ビ") ;
        hira2kana.Add("ぶ",  "ブ") ;
        hira2kana.Add("べ",  "ベ") ;
        hira2kana.Add("ぼ",  "ボ") ;
          
        hira2kana.Add("ぱ",  "パ") ;
        hira2kana.Add("ぴ",  "ピ") ;
        hira2kana.Add("ぷ",  "プ") ;
        hira2kana.Add("ぺ",  "ペ") ;
        hira2kana.Add("ぽ",  "ポ") ;
         
        //------------------------------------
        hira2kana.Add("ま",  "マ") ;
        hira2kana.Add("み",  "ミ") ;
        hira2kana.Add("む",  "ム") ;
        hira2kana.Add("め",  "メ") ;
        hira2kana.Add("も",  "モ") ;
         
        //------------------------------------
         
        hira2kana.Add("や",  "ヤ") ;
        hira2kana.Add("ゆ",  "ユ") ;
        hira2kana.Add("よ",  "ヨ") ;
        
        hira2kana.Add("ゃ",  "ャ") ;
        hira2kana.Add("ゅ",  "ュ") ;
        hira2kana.Add("ょ",  "ョ") ;
         

        hira2kana.Add("ら",  "ラ") ;
        hira2kana.Add("り",  "リ") ;
        hira2kana.Add("る",  "ル") ;
        hira2kana.Add("れ",  "レ") ;
        hira2kana.Add("ろ",  "ロ") ;

        hira2kana.Add("わ",  "ワ") ;
        hira2kana.Add("ゑ",  "ヱ") ;
        hira2kana.Add("を",  "ヲ") ;
        hira2kana.Add("ん",  "ン") ;

    }



    //--------------------------------------------------------------------
    static public string  R2K( string input ) {
        return Parse(input, roma2kana) ;
    }
    static public string  K2R( string input ) {
        return Parse(input, kana2roma) ;
    }
    static public string  H2K( string input ) {
        return Parse(input, hira2kana) ;
    }
    //--------------------------------------------------------------------
    // 変換器  ：   今は効率は考えない
    //--------------------------------------------------------------------
    static private string  Parse( string input , Hashtable  table ) {

        string output = ""; 
        bool converted;

        int[] substrLengList = new int[3] { 3, 2, 1 };
        
        while(input.Length != 0){
            converted=false;
            
            foreach ( int substrLeng in substrLengList ){
                if(input.Length < substrLeng){
                    continue;
                }
                string substr  = input.Substring(0, substrLeng);
                if( table[substr] != null ){
                    output = output + (String)table[substr] ;
                    input = input.Remove(0, substrLeng );
                    converted=true;
                    break;
                }
            }
            
            if(!converted){
                string head  = input.Substring(0, 1);
                output = output + head ;
                input = input.Remove(0, 1 );
            }
             
        }    
        
        return output ;
    }

} 

    
//-----------------------------------------------------------------------------
} // end of namespace  HYAM.Lingua


