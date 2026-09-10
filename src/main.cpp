#include "Nau7802.h"
#include "Calibrator.h"

const int BUTTON_PIN = 9;
volatile bool buttonFlag = false;

NauAdc _adc;
Calibrator _calibrator;
uint64_t _startMills = 0;
uint32_t _intervalLength = 1000; // 1 second
// unsigned long _elapsedTime = 0;

void IRAM_ATTR handleButtonPress()
{
    buttonFlag = !buttonFlag;
}

void setup()
{
    Serial.begin(115200);
    delay(500);
    Serial.println("Starting PearlHq");

    // Set up pin with internal pull-up resistor
    pinMode(BUTTON_PIN, INPUT_PULLUP);
    attachInterrupt(digitalPinToInterrupt(9), handleButtonPress, FALLING);

    _adc.initialize();
    DualChannelReadings readings = _adc.getReadings();
    _calibrator.ZeroCalibratedReading(readings);
    Serial.printf("Initial readings: CH0=%d, CH1=%d\n", readings.channel0, readings.channel1);
    Serial.println("Calibration complete. Press the button to get calibrated readings.");

    _startMills = millis();
}

void loop()
{
    if (buttonFlag)
    {
        Serial.println("Button pressed! Getting calibrated readings...");
        DualChannelReadings readings = _adc.getReadings();
        _calibrator.ZeroCalibratedReading(readings);
        Serial.printf("Re-calibrated readings: CH0=%d, CH1=%d\n", readings.channel0, readings.channel1);
        Serial.println("Calibration complete. Press the button to get calibrated readings.");
    }

    if ((millis() - _startMills) >= _intervalLength)
    {
        _startMills = millis();

        DualChannelReadings readings = _adc.getReadings();
        Serial.printf("Raw readings: CH0=%d, CH1=%d\n", readings.channel0, readings.channel1);
        uint32_t calculatedMg = _calibrator.Calculate(readings);
        Serial.printf("----------------------\n");
        Serial.printf("Value: %d g\n", calculatedMg);
        Serial.printf("----------------------\n");
        buttonFlag = false; // Reset the flag after handling the button press
        delay(1000);        // Delay to avoid flooding the serial output
    }
}