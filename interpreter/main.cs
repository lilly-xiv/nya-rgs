using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
class Program {
  //Commands:
  // @ Print Held Memory as string. e.g. @0. prints address 0
  // ~ Clear Held Memory e.g. ~0. for pos 0 or ~*. for all
  // | Goto Line:Pos unconditionally e.g. |3:3.
  // ; Input into Memory at pos e.g. ;3.
  // ' Load string into memory e.g. '0:Hello World. will load hello world in position 0
  // ^ Load int into memory e.g. ^0:1 will load 1 into position 0
  // ? Goto Line:Pos if Memory == value e.g. ?5:0:99:4. will go to line 5 character 0 if memory address 99 is 4
  // ?> will compare memory1 to memory2 instead of memory and value so ?>0:0:1:2 will go to the start if memory address 1 and 2 are equal
  // + Incriment address e.g. +5:1. adds 1 to address 5
  // - Decrement address
  // * Multiply address. *5:2. multiplies address 5 by 2
  // / Divide address
  // arithematic operator followed by > will work as followed
  // operator>address1:address2 e.g. +>1:2 will add addresses 1 and 2 and put it into address 1
  // _ Halt
  // . End command
  // & Convert address to int e.g. &0. on failure will crash
  // % Convert address to string e.g. %0. on failure will crash
  // ! Convert address to character
  // = Join addresses together and stores in the first address e.g. =0:1 when address 0 is 'c' and address 1 is 'a' it will set address 0 to 'ca'
  // > Compares two addresses in size and sets the third one to the result e.g. >0:2:3 checks if address 0 is bigger than address 2 and stores it in address 3
  // < Waits a certain number of ms e.g. <50. waits 50 ms (0.05s)
  public static void Main(string[] args) {
    Console.WriteLine("File Path: ");
    string file = Console.ReadLine();
    Parse(File.ReadAllLines(file));
  }
  public static Object[] Memory = new Object[500];
  public static void Parse(string[] code) {
    char lastcmd = ' ';
    char extracmd = ' ';
    char last = ' ';
    string loading = "";
    string loaded = "";
    uint address = 0;
    int colons = 0;
    (int,int,int,int) values = (0,0,0,0);
    int i = 0;
    int j = 0;
    string temp = "";
    for(i = 0; i < code.Length; i++){
      string line = "  "+code[i];
      for(j = 0; j < line.Length; j++){
        char c = line[j];
        if(c == '_') { j = 99999; i = 99999; break; }
        if (lastcmd == ' ') {
        char[] commands = {'@', '~', '|', ';', '\'', '^', '?', '+', '-', '*', '/', '&', '%', '!', '=', '>', '<'};
        if(commands.Contains(c)){
          lastcmd = c;
          extracmd = ' ';
          loading = "";
          colons = 0;
        }
        else{
        }
      } else {
        switch (lastcmd) {
        case '<':
          if(c == '.'){
            loaded = loading;
            address = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            Thread.Sleep((int)address);
          } 
          else{
            loading += c;
          }
          break;
        case '@':
          if(c == '.'){
            loaded = loading;
            address = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            string s = Memory[address].ToString().Replace(@"\n", "\n");
            Console.Write(s);
          } 
          else{
            loading += c;
          }
          break;
        case '~':
          if(c == '.'){
            loaded = loading;
            loading = "";
            lastcmd = ' ';
            if(loaded == "*") Memory = new object[500];
            else {
              address = UInt32.Parse(loaded);
              Memory[address] = 0;
            }
          }
          else{
            loading += c;
          }
          break;
        case '|':
          if(c == ':'){
            loaded = loading;
            values.Item1 = Int32.Parse(loaded);
            loading = "";
          }
          else if(c == '.'){
            loaded = loading;
            address = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            last = ' ';
            loaded = "";
            extracmd = ' ';
            i = values.Item1-1;
            j = (int)address;
          }
          else{
            loading += c;
          }
          break;
        case ';':
          if(c == '.'){
            loaded = loading;
            address = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            Memory[address] = Console.ReadLine();
          } 
          else{
            loading += c;
          }
          break;
        case '\'':
          if(c == ':'){
              loaded = loading;
              values = (Int32.Parse(loaded),0,0,0);
              loading = "";
          } 
          else if(c == '.'){
            loaded = loading;
            loading = "";
            lastcmd = ' ';
            Memory[values.Item1] = loaded.Replace("/;", @":");
          } 
          else{
            loading += c;
          }
          break;
        case '^':
          if(c == ':'){
            loaded = loading;
            values = (Int32.Parse(loaded),0,0,0);
            loading = "";
          } 
          else if(c == '.'){
            loaded = loading;
            loading = "";
            lastcmd = ' ';
            Memory[values.Item1] = Int32.Parse(loaded);
          } 
          else{
            loading += c;
          }
          break;
        case '?':
          if(c == ':'){
            loaded = loading;
            switch(colons){
              case 0: values.Item1 = Int32.Parse(loaded); break;
              case 1: values.Item2 = Int32.Parse(loaded); break;
              case 2: values.Item3 = Int32.Parse(loaded); break;
            }
            loading = "";
            colons++;
          }
          else if(c == '.'){
            loaded = loading;
            address = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            last = ' ';
            loaded = "";
            extracmd = ' ';
            if(Memory[values.Item3].ToString()==Memory[address].ToString()){
              i = values.Item1-1;
              j = values.Item2;
            }
          }
          else{
            loading += c;
          }
          break;
        case '&':
          if(c == '.'){
            loaded = loading;
            address = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            if(!Int32.TryParse(Memory[address].ToString(), out int val)){
              Memory[address] = (int)(Memory[address].ToString().ToCharArray()[0]);
            }
            else Memory[address] = Int32.Parse(Memory[address].ToString());
          } 
          else{
            loading += c;
          }
          break;
        case '%':
          if(c == '.'){
            loaded = loading;
            address = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            Memory[address] = Memory[address].ToString();
          } 
          else{
            loading += c;
          }
          break;
        case '!':
          if(c == '.'){
            loaded = loading;
            address = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            Memory[address] = (char)((int)Memory[address]);
          } 
          else{
            loading += c;
          }
          break;
          
        case '=':
          if(c == ':'){
            loaded = loading;
            address = UInt32.Parse(loaded); 
            loading = "";
          } 
          else if(c == '.'){
            loaded = loading;
            uint value = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            Memory[address] = Memory[address].ToString()+Memory[value].ToString();
          }
          else{
            loading += c;
          }
          break;
          
        case '>':
          if(c == ':'){
            loaded = loading;
            switch(colons){
              case 0: values.Item1 = Int32.Parse(loaded); break;
              case 1: values.Item2 = Int32.Parse(loaded); break;
              }
            colons++;
            loading = "";
          } 
          else if(c == '.'){
            loaded = loading;
            uint value = UInt32.Parse(loaded);
            loading = "";
            lastcmd = ' ';
            Memory[value] = (int)Memory[values.Item1] > (int)Memory[values.Item2] ? 1 : 0; 
          } 
          else{
            loading += c;
          }
          break;
        case '+':
          if(c == '>'){
            extracmd = '>';
            loading = "";
          } 
          else if(extracmd != '>'){
            if(c == ':'){
              loaded = loading;
              loading = "";
              address = UInt32.Parse(loaded);
            } 
            else if(c == '.'){
              loaded = loading;
              int value = Int32.Parse(loaded);
              loading = "";
              lastcmd = ' ';
              Memory[address] = (int)Memory[address]+value;
            } 
            else loading += c;
          }
          else{
            if(c == ':'){
              loaded = loading;
              loading = "";
              address = UInt32.Parse(loaded);
            } 
            else if(c == '.'){
              loaded = loading;
              int value = Int32.Parse(loaded);
              loading = "";
              lastcmd = ' ';
              Memory[address] = (int)Memory[address]+(int)Memory[value];
            } 
            else loading += c;
          }
          break;
        case '-':
          if(c == '>'){
            extracmd = '>';
            loading = "";
          } 
          else if(extracmd != '>'){
            if(c == ':'){
              loaded = loading;
              loading = "";
              address = UInt32.Parse(loaded);
            } 
            else if(c == '.'){
              loaded = loading;
              int value = Int32.Parse(loaded);
              loading = "";
              lastcmd = ' ';
              Memory[address] = (int)Memory[address]-value;
            } 
            else loading += c;
          }
          else{
            if(c == ':'){
              loaded = loading;
              loading = "";
              address = UInt32.Parse(loaded);
            } 
            else if(c == '.'){
              loaded = loading;
              int value = Int32.Parse(loaded);
              loading = "";
              lastcmd = ' ';
              Memory[address] = (int)Memory[address]-(int)Memory[value];
            } 
            else loading += c;
          }
          break;
        case '*':
          if(c == '>'){
            extracmd = '>';
            loading = "";
          } 
          else if(extracmd != '>'){
            if(c == ':'){
              loaded = loading;
              loading = "";
              address = UInt32.Parse(loaded);
            } 
            else if(c == '.'){
              loaded = loading;
              int value = Int32.Parse(loaded);
              loading = "";
              lastcmd = ' ';
              Memory[address] = (int)Memory[address]*value;
            } 
            else loading += c;
          }
          else{
            if(c == ':'){
              loaded = loading;
              loading = "";
              address = UInt32.Parse(loaded);
            } 
            else if(c == '.'){
              loaded = loading;
              int value = Int32.Parse(loaded);
              loading = "";
              lastcmd = ' ';
              Memory[address] = (int)Memory[address]*(int)Memory[value];
            } 
            else loading += c;
          }
          break;
        
        case '/':
          if(c == '>'){
            extracmd = '>';
            loading = "";
          } 
          else if(extracmd != '>'){
            if(c == ':'){
              loaded = loading;
              loading = "";
              address = UInt32.Parse(loaded);
            } 
            else if(c == '.'){
              loaded = loading;
              int value = Int32.Parse(loaded);
              loading = "";
              lastcmd = ' ';
              Memory[address] = (int)Memory[address]/value;
            } 
            else loading += c;
          }
          else{
            if(c == ':'){
              loaded = loading;
              loading = "";
              address = UInt32.Parse(loaded);
            } 
            else if(c == '.'){
              loaded = loading;
              int value = Int32.Parse(loaded);
              loading = "";
              lastcmd = ' ';
              Memory[address] = (int)Memory[address]/(int)Memory[value];
            } 
            else loading += c;
          }
          break;
        }
          
        }
        last = c;
        }
    }
  }
}
