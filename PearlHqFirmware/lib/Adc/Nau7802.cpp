#include "Nau7802.h"

NauAdc::NauAdc()
{
}

bool NauAdc::initialize()
{
    if (!_nau7802.begin())
    {
        Serial.println("Failed to find NAU7802");
        return false;
    }

    // volatage 3V
    // gain 128x
    // rate 320 SPS

    Serial.println("Nau7802: Setting LDO voltage to 3V");
    _nau7802.setLDO(NAU7802_3V0);
    Serial.printf("Nau7802: LDO voltage set to %s\n", NauAdc::getLDOString(_nau7802.getLDO()));

    NAU7802_Gain gain = NAU7802_GAIN_128;
    Serial.println("Nau7802: Setting gain to 128x");
    _nau7802.setGain(NAU7802_GAIN_128);
    Serial.printf("Nau7802: Gain set to %s\n", NauAdc::getGainString(_nau7802.getGain()));

    NAU7802_SampleRate rate = NAU7802_RATE_320SPS;
    Serial.println("Nau7802: Setting conversion rate to 320 SPS");
    _nau7802.setRate(NAU7802_RATE_320SPS);
    Serial.printf("Nau7802: Conversion rate set to %s\n", NauAdc::getRateString(_nau7802.getRate()));

    _initialized = true;
    return true;
}

uint32_t NauAdc::getChannelReading(int channel)
{
    _nau7802.setChannel(channel);
    // Take 10 readings to flush out readings
    for (uint8_t i = 0; i < 10; i++)
    {
        while (!_nau7802.available())
        {
            delay(1);
        }
        _nau7802.read();
    }
    uint32_t reading = 0;
    for (uint8_t i = 0; i < _sampleSize; i++)
    {
        while (!_nau7802.available())
        {
            delay(1);
        }
        reading += _nau7802.read();
    }
    reading /= _sampleSize;
    return reading;
}

DualChannelReadings NauAdc::getReadings()
{
    if (!_initialized)
    {
        Serial.println("Nau7802: Not initialized");
        return DualChannelReadings();
    }

    uint32_t chan0 = getChannelReading(0);
    uint32_t chan1 = getChannelReading(1);

    return DualChannelReadings{chan0, chan1};
}

const char *NauAdc::getLDOString(NAU7802_LDOVoltage ldo)
{
    switch (ldo)
    {
    case NAU7802_4V5:
        return "4.5V";
    case NAU7802_4V2:
        return "4.2V";
    case NAU7802_3V9:
        return "3.9V";
    case NAU7802_3V6:
        return "3.6V";
    case NAU7802_3V3:
        return "3.3V";
    case NAU7802_3V0:
        return "3.0V";
    case NAU7802_2V7:
        return "2.7V";
    case NAU7802_2V4:
        return "2.4V";
    case NAU7802_EXTERNAL:
        return "External";
    default:
        return "Unknown";
    }
}

const char *NauAdc::getGainString(NAU7802_Gain gain)
{
    switch (gain)
    {
    case NAU7802_GAIN_1:
        return "1x";
    case NAU7802_GAIN_2:
        return "2x";
    case NAU7802_GAIN_4:
        return "4x";
    case NAU7802_GAIN_8:
        return "8x";
    case NAU7802_GAIN_16:
        return "16x";
    case NAU7802_GAIN_32:
        return "32x";
    case NAU7802_GAIN_64:
        return "64x";
    case NAU7802_GAIN_128:
        return "128x";
    default:
        return "Unknown";
    }
}

const char *NauAdc::getRateString(NAU7802_SampleRate rate)
{
    switch (rate)
    {
    case NAU7802_RATE_10SPS:
        return "10 SPS";
    case NAU7802_RATE_20SPS:
        return "20 SPS";
    case NAU7802_RATE_40SPS:
        return "40 SPS";
    case NAU7802_RATE_80SPS:
        return "80 SPS";
    case NAU7802_RATE_320SPS:
        return "320 SPS";
    default:
        return "Unknown";
    }
}
