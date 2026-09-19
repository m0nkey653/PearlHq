#ifndef CALIBRATOR_H
#define CALIBRATOR_H

#include "../Adc/Nau7802.h"

class Calibrator
{
public:
    Calibrator() = default;
    ~Calibrator() = default;

    void ZeroCalibratedReading(DualChannelReadings readings);
    uint32_t Calculate(DualChannelReadings readings);

private:
    uint32_t _zeroOffsetChannel0 = 0;
    uint32_t _zeroOffsetChannel1 = 0;
};

#endif // CALIBRATOR_H