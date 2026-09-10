#include "Calibrator.h"

const float unitsPerGram = 373.576f;
const float unitsPerMilligram = .373576f;

void Calibrator::ZeroCalibratedReading(DualChannelReadings readings)
{
    _zeroOffsetChannel0 = readings.channel0;
    _zeroOffsetChannel1 = readings.channel1;
    Serial.printf("Zero calibration complete. Offsets: CH0=%d, CH1=%d\n", _zeroOffsetChannel0, _zeroOffsetChannel1);
}

/*
500g = 186788
1g = 373.576
1mg = 0.367576
*/

uint32_t Calibrator::Calculate(DualChannelReadings readings)
{
    readings.channel0 -= _zeroOffsetChannel0;
    readings.channel1 -= _zeroOffsetChannel1;
    Serial.printf("Calibrated readings: CH0=%d, CH1=%d\n", readings.channel0, readings.channel1);
    int32_t readingSum = readings.channel0 + readings.channel1;
    Serial.printf("Sum of calibrated readings: %d\n", readingSum);
    Serial.printf("Units per gram: %.6f\n", unitsPerGram);
    readingSum = max(readingSum, (int32_t)(0));
    Serial.printf("Sum of calibrated readings (zeroed): %d\n", readingSum);
    return static_cast<uint32_t>(readingSum / unitsPerGram);
}