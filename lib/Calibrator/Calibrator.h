#ifndef CALIBRATOR_H
#define CALIBRATOR_H

#include "../Adc/Nau7802.h"

class Calibrator
{
public:
    Calibrator() = default;
    ~Calibrator() = default;

    void ZeroCalibratedReading(DualChannelReadings readings);

private:
};

#endif // CALIBRATOR_H