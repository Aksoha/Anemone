#pragma once
#include <complex>
#include <unordered_map>

using HeatingSystemParameters = std::unordered_map<double, double>;

class HeatingSystem
{
private:
	HeatingSystemParameters _resistanceFrequency;
	HeatingSystemParameters _inductanceFrequency;
	HeatingSystemParameters _resistanceTemperature;
	HeatingSystemParameters _inductanceTemperature;

public:
	HeatingSystem(const HeatingSystemParameters& resistanceFrequency, const HeatingSystemParameters& inductanceFrequency,
				  const HeatingSystemParameters& resistanceTemperature, const HeatingSystemParameters& inductanceTemperature);


	[[nodiscard]] double Resistance(double frequency, double temperature) const;
	[[nodiscard]] double Inductance(double frequency, double temperature) const;
	[[nodiscard]] std::complex<double> Impedance(double frequency, double temperature) const;
};
