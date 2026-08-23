#include "Nau7802.h"

const int BUTTON_PIN = 9;
volatile bool buttonFlag = false;

NauAdc _adc;

void IRAM_ATTR handleButtonPress()
{
    buttonFlag = !buttonFlag;
}

void setup()
{
    Serial.begin(115200);
    delay(500);
    Serial.println("Starting PearlHq");
    _adc.initialize();

    // Set up pin with internal pull-up resistor
    pinMode(BUTTON_PIN, INPUT_PULLUP);
    attachInterrupt(digitalPinToInterrupt(9), handleButtonPress, FALLING);
}

void loop()
{
    //--------------
    // results
    //--------------
    if (buttonFlag)
    {
        DualChannelReadings readings = _adc.getReadings();
        Serial.printf("Button pressed! Readings: CH0=%d, CH1=%d\n", readings.channel0, readings.channel1);
        buttonFlag = false; // Reset the flag after handling the button press
    }
}