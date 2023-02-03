#pragma once
#include <exception>

class SolutionNotFound : public std::exception
{
private:
	const char* message;
public:
	double MaxInductance;
	SolutionNotFound(const char* msg);
	SolutionNotFound(const char* msg, double maxInductance);
	[[nodiscard]] char* What() const;
};
