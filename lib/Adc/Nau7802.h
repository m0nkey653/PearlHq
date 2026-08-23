#ifndef NAU7802_H
#define NAU7802_H

#include <cstdint>
#include <Adafruit_NAU7802.h>
#include <string>

struct DualChannelReadings
{
    int32_t channel0;
    int32_t channel1;
};

class NauAdc
{
public:
    NauAdc();
    bool initialize();
    DualChannelReadings getReadings();

private:
    Adafruit_NAU7802 _nau7802;
    bool _initialized = false;

    static std::string getRateString(NAU7802_SampleRate rate);
    static std::string getLDOString(NAU7802_LDOVoltage ldo);
    static std::string getGainString(NAU7802_Gain gain);
};

#endif // NAU7802_H