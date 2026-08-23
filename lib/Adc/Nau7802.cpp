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

    NAU7802_LDOVoltage ldoVoltage = NAU7802_3V0;
    Serial.print("Nau7802: Setting LDO voltage to 3V");
    _nau7802.setLDO(ldoVoltage);
    Serial.print("Nau7802: LDO voltage set to ");
    Serial.println(NauAdc::getLDOString(ldoVoltage).c_str());

    NAU7802_Gain gain = NAU7802_GAIN_128;
    Serial.print("Nau7802: Setting gain to 128x");
    _nau7802.setGain(gain);
    Serial.print("Nau7802: Gain set to ");
    Serial.println(NauAdc::getGainString(_nau7802.getGain()).c_str());

    NAU7802_SampleRate rate = NAU7802_RATE_320SPS;
    Serial.print("Nau7802: Setting conversion rate to 320 SPS");
    _nau7802.setRate(rate);
    Serial.print("Nau7802: Conversion rate set to ");
    Serial.println(NauAdc::getRateString(_nau7802.getRate()).c_str());

    _initialized = true;
    return true;
}

DualChannelReadings NauAdc::getReadings()
{
    if (!_initialized)
    {
        Serial.println("Nau7802: Not initialized");
        return DualChannelReadings();
    }

    int32_t chan0, chan1;

    // Switch channel
    _nau7802.setChannel(0);
    // Take 10 readings to flush out readings
    for (uint8_t i = 0; i < 10; i++)
    {
        while (!_nau7802.available())
            delay(1);
        _nau7802.read();
    }
    // Take actual reading
    while (!_nau7802.available())
    {
        delay(1);
    }
    chan0 = _nau7802.read();

    //--------------
    // CHAN 1
    //--------------
    // Switch channel
    _nau7802.setChannel(1);
    // Take 10 readings to flush out readings
    for (uint8_t i = 0; i < 10; i++)
    {
        while (!_nau7802.available())
            delay(1);
        _nau7802.read();
    }
    // Take actual reading
    while (!_nau7802.available())
    {
        delay(1);
    }
    chan1 = _nau7802.read();

    return DualChannelReadings{chan0, chan1};
}

std::string NauAdc::getLDOString(NAU7802_LDOVoltage ldo)
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

std::string NauAdc::getGainString(NAU7802_Gain gain)
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

std::string NauAdc::getRateString(NAU7802_SampleRate rate)
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
