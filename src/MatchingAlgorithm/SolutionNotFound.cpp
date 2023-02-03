#include "SolutionNotFound.h"

#include <cstring>

SolutionNotFound::SolutionNotFound(const char* msg)
: message(msg), MaxInductance(NAN)
{
}

SolutionNotFound::SolutionNotFound(const char* msg, double maxInductance)
: message(msg), MaxInductance(maxInductance)
{
}

char* SolutionNotFound::What() const
{
	return _strdup(message);
}
