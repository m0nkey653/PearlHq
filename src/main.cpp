#include "Nau7802.h"
#include "Calibrator.h"

#include <WiFi.h>
#include <HTTPClient.h>
#include "secrets.h"

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

    wl_status_t status = WiFi.begin(Secrets::SSID, Secrets::PASSWORD);
    while (status != WL_CONNECTED)
    {
        switch (status)
        {
        case WL_NO_SSID_AVAIL:
            Serial.println("SSID not available");
            break;
        case WL_CONNECT_FAILED:
            Serial.println("Connection failed");
            break;
        case WL_IDLE_STATUS:
            Serial.println("WiFi idle");
            break;
        case WL_DISCONNECTED:
            Serial.println("WiFi disconnected");
            break;
        case WL_SCAN_COMPLETED:
            Serial.println("WiFi scan completed");
            break;
        default:
            Serial.println("Connecting...");
            break;
        }
        delay(500);
        Serial.print(".");
        status = WiFi.status();
    }
    Serial.println("\nConnected to WiFi!");
    WiFi.setAutoReconnect(true);
    auto ip = WiFi.broadcastIP();
    Serial.printf("Broadcast IP: %s\n", ip.toString().c_str());

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