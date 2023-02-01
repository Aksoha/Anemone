#pragma once
#include <unordered_map>

#include "HeatingSystemData.h"

using HeatingSystemParameters = std::unordered_map<double, double>;

class HeatingSystem
{
private:
	HeatingSystemParameters _resistanceFrequency;
	HeatingSystemParameters _inductanceFrequency;
	HeatingSystemParameters _resistanceTemperature;
	HeatingSystemParameters _inductanceTemperature;

public:
	HeatingSystem(const std::vector<HeatingSystemData>& frequency, const std::vector<HeatingSystemData>& temperature);
	[[nodiscard]] double Resistance(double frequency, double temperature) const;
	[[nodiscard]] double Inductance(double frequency, double temperature) const;
	[[nodiscard]] double Impedance(double frequency, double temperature) const;
};
