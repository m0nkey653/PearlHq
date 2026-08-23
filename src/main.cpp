#include "Nau7802.h"

NauAdc _adc;

void setup()
{
    Serial.begin(115200);
    delay(500);
    Serial.println("Starting PearlHq");
    _adc.initialize();
}

void loop()
{
    DualChannelReadings readings = _adc.getReadings();

    //--------------
    // results
    //--------------
    Serial.print(readings.channel0);
    Serial.print(",");
    Serial.println(readings.channel1);
}