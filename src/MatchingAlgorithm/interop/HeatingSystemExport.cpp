#pragma once
#include "export.h"
#include "../HeatingSystem.h"


DLL_EXPORT HeatingSystem* CreateHeatingSystem(const HeatingSystemData* frequency, const HeatingSystemData* temperature, const long frequencyLength, const long temperatureLength)
{
	std::vector<HeatingSystemData> _frequency;
	std::vector<HeatingSystemData> _temperature;

	_frequency.reserve(frequencyLength);
	for (auto i = 0; i < frequencyLength; i++)
	{
		_frequency.push_back(frequency[i]);
	}

	_temperature.reserve(temperatureLength);
	for (auto i = 0; i < temperatureLength; i++)
	{
		_temperature.push_back(temperature[i]);
	}

	return new HeatingSystem(_frequency, _temperature);
}

DLL_EXPORT void DisposeHeatingSystem(const HeatingSystem* heatingSystem)
{
	delete heatingSystem;
}

DLL_EXPORT double Resistance(const HeatingSystem* heatingSystem,  double frequency, const double temperature)
{
	return heatingSystem->Resistance(frequency, temperature);
}

DLL_EXPORT double Inductance(const HeatingSystem* heatingSystem,  double frequency, const double temperature)
{
	return heatingSystem->Inductance(frequency, temperature);
}

DLL_EXPORT double Impedance(const HeatingSystem* heatingSystem,  double frequency, const double temperature)
{
	return heatingSystem->Impedance(frequency, temperature);
}