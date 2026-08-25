#include "Calibrator.h"

void Calibrator::ZeroCalibratedReading(DualChannelReadings readings)
{
    _zeroOffsetChannel0 = readings.channel0;
    _zeroOffsetChannel1 = readings.channel1;
}

uint32_t Calibrator::Calculate(DualChannelReadings readings)
{
    readings.channel0 -= _zeroOffsetChannel0;
    readings.channel1 -= _zeroOffsetChannel1;
    return readings.channel0 + readings.channel1;
}