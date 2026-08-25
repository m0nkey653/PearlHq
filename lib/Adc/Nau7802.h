#ifndef NAU7802_H
#define NAU7802_H

#include <cstdint>
#include <Adafruit_NAU7802.h>
#include <string>

struct DualChannelReadings
{
    uint32_t channel0;
    uint32_t channel1;
};

class NauAdc
{
public:
    NauAdc();
    ~NauAdc() = default;
    bool initialize();
    DualChannelReadings getReadings();

private:
    Adafruit_NAU7802 _nau7802;
    bool _initialized = false;
    uint8_t _sampleSize = 15;

    static const char *getRateString(NAU7802_SampleRate rate);
    static const char *getLDOString(NAU7802_LDOVoltage ldo);
    static const char *getGainString(NAU7802_Gain gain);

    uint32_t getChannelReading(int channel);
};

#endif // NAU7802_H