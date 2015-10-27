import processing.serial.*;
import oscP5.*;
import netP5.*;

import java.util.regex.Pattern;
import java.util.regex.Matcher;

int sensorPin = 0;
Serial myPort;
String fileName; 

Table dataTable;
int numReadings = 100;
int readingCounter = 1;
int oscPort = 12000;
int serialSpeed = 9600;

OscP5 oscConnection;
NetAddress myRemoteLocation;

void setup()
{

  String portName = Serial.list()[1];
  myPort = new Serial(this, portName, serialSpeed);
  
  oscConnection = new OscP5(this, oscPort);
  myRemoteLocation = new NetAddress("127.0.0.1", 3200);
  myPort.readStringUntil('\n'); 
  
}

void draw()
{
  Pattern p = Pattern.compile("\\d+");
  String potential = myPort.readStringUntil('\n');
  if(potential != null){
    Matcher m = p.matcher(potential);
    float depthValue;
    float roll; float pitch; float yaw;
    
    //Order: depthValue, roll, pitch, yaw
    //Must be kept in sync with arduino sending values
   
    //depthValue
    m.find();
    depthValue = 5 * Float.parseFloat(m.group());
    
    m.find();
    roll = 5 * Float.parseFloat(m.group());
    m.find();
    pitch = 5 * Float.parseFloat(m.group());
    m.find();
    yaw = 5 * Float.parseFloat(m.group());

    println("depth");
    println(depthValue);
    println("roll");
    println(roll);
    println("pitch");
    println(pitch);
    println("yaw");
    println(yaw);
    
    OscMessage oscMess = new OscMessage("/arduinoData");
    oscMess.add(depthValue);
    oscMess.add(roll);
    oscMess.add(pitch);
    oscMess.add(yaw);
    
    oscConnection.send(oscMess, myRemoteLocation);

  }
}