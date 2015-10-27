
#include <Wire.h>
#include <SPI.h>
#include <Adafruit_LSM9DS0.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_Simple_AHRS.h>


//i2c
Adafruit_LSM9DS0 lsm = Adafruit_LSM9DS0(1000);

//Create AHRS algorithm using instance accel and magnetometer
Adafruit_Simple_AHRS ahrs(&lsm.getAccel(), &lsm.getMag());

int sensorPin = A0;
int sensorValue = 0;


void setupSensor()
{
  // 1.) Set the accelerometer range
  lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_2G);
  //lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_4G);
  //lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_6G);
  //lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_8G);
  //lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_16G);
  
  // 2.) Set the magnetometer sensitivity
  lsm.setupMag(lsm.LSM9DS0_MAGGAIN_2GAUSS);
  //lsm.setupMag(lsm.LSM9DS0_MAGGAIN_4GAUSS);
  //lsm.setupMag(lsm.LSM9DS0_MAGGAIN_8GAUSS);
  //lsm.setupMag(lsm.LSM9DS0_MAGGAIN_12GAUSS);

  // 3.) Setup the gyroscope
  lsm.setupGyro(lsm.LSM9DS0_GYROSCALE_245DPS);
  //lsm.setupGyro(lsm.LSM9DS0_GYROSCALE_500DPS);
  //lsm.setupGyro(lsm.LSM9DS0_GYROSCALE_2000DPS);

}


void setup() {
  while (!Serial); // flora & leonardo
  
  Serial.begin(9600);
  // Try to initialise and warn if we couldn't detect the chip
  if (!lsm.begin())
  {
    while (1);
  }

  setupSensor();
}

void loop() {
  // read values from both sensors
  sensorValue = analogRead(sensorPin);
  sensors_vec_t orientation;
  
  
  lsm.read();
  if(ahrs.getOrientation(&orientation)){
    if(!Serial.available()){
      //Order: depthValue, roll, pitch, yaw
      //Must be kept in sync with processing parser
      Serial.print("depthValue: ");  Serial.print(sensorValue);
      Serial.print("roll: "); Serial.print(orientation.roll);        Serial.print(" ");
      Serial.print("pitch: "); Serial.print(orientation.pitch);      Serial.print(" ");
      Serial.print("yaw: "); Serial.print(orientation.heading);       Serial.println(" ");
           
      Serial.flush();
    }
  }

}
