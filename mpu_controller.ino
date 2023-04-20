#include "Wire.h"

const int MPU_ADDR1 = 0X68;
const int MPU_ADDR2 = 0X69;

int16_t accelerometer_x, accelerometer_y, accelerometer_z;
int16_t gyro_x, gyro_y, gyro_z;
int16_t temperature;

char tmp_str[7];

char* convert_int16_to_str(int16_t i) {
  sprintf(tmp_str, "%d", i);
  return tmp_str;
}

void setup() {
  Serial.begin(9600);
  Wire.begin();
  Wire.beginTransmission(MPU_ADDR1);
  Wire.write(0x6B);
  Wire.write(0);
  Wire.endTransmission(true);

  Wire.begin();
  Wire.beginTransmission(MPU_ADDR2);
  Wire.write(0x6B);
  Wire.write(0); 
  Wire.endTransmission(true);
}

void loop() {
  Wire.beginTransmission(MPU_ADDR1);
  Wire.write(0x3B);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU_ADDR1, 7 * 2, true);

  accelerometer_x = Wire.read() << 8 | Wire.read();
  accelerometer_y = Wire.read() << 8 | Wire.read();
  accelerometer_z = Wire.read() << 8 | Wire.read();

  Serial.print(convert_int16_to_str(accelerometer_x));
  Serial.print(",");
  Serial.print(convert_int16_to_str(accelerometer_y));
  Serial.print(",");
  Serial.print(convert_int16_to_str(accelerometer_z));
  Serial.print("|");

  Wire.beginTransmission(MPU_ADDR2);
  Wire.write(0x3B);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU_ADDR2, 7 * 2, true);

  accelerometer_x = Wire.read() << 8 | Wire.read();
  accelerometer_y = Wire.read() << 8 | Wire.read();
  accelerometer_z = Wire.read() << 8 | Wire.read();

  Serial.print(convert_int16_to_str(accelerometer_x));
  Serial.print(",");
  Serial.print(convert_int16_to_str(accelerometer_y));
  Serial.print(",");
  Serial.print(convert_int16_to_str(accelerometer_z));
  Serial.println();
  delay(200);
}