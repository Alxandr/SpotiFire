#include "stdafx.h"
#include "Exceptions.h"

NotLoadedException::NotLoadedException() : Exception() {
}
NotLoadedException::NotLoadedException(String ^msg) : Exception(msg) {
}