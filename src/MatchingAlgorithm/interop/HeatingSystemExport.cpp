#include "HeatingSystemExport.h"


DLL_EXPORT HeatingSystem* HeatingSystem_Create(const HeatingSystemData* frequency, const HeatingSystemData* temperature,
											   const long frequencyLength, const long temperatureLength)
{
	std::vector<HeatingSystemData> _frequency;
	std::vector<HeatingSystemData> _temperature;

	_frequency.reserve(frequencyLength);
	for (auto i = 0; i < frequencyLength; i++)
		_frequency.push_back(frequency[i]);

	_temperature.reserve(temperatureLength);
	for (auto i = 0; i < temperatureLength; i++)
		_temperature.push_back(temperature[i]);

	return new HeatingSystem(_frequency, _temperature);
}

DLL_EXPORT void HeatingSystem_Dispose(const HeatingSystem* heatingSystem)
{
	delete heatingSystem;
}

DLL_EXPORT double HeatingSystem_Resistance(const HeatingSystem* heatingSystem, const double frequency,
										   const double temperature)
{
	return heatingSystem->Resistance(frequency, temperature);
}

DLL_EXPORT double HeatingSystem_Inductance(const HeatingSystem* heatingSystem, const double frequency,
										   const double temperature)
{
	return heatingSystem->Inductance(frequency, temperature);
}

DLL_EXPORT double HeatingSystem_Impedance(const HeatingSystem* heatingSystem, const double frequency,
										  const double temperature)
{
	return heatingSystem->Impedance(frequency, temperature);
}
