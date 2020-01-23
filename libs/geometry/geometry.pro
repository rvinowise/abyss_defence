#QT = core gui widgets

TEMPLATE = lib
DEFINES += GEOMETRY_LIBRARY

CONFIG += c++11


# You can also make your code fail to compile if it uses deprecated APIs.
# In order to do so, uncomment the following line.
# You can also select to disable deprecated APIs only up to a certain version of Qt.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

SOURCES += \
    Polygon_splitter.cpp \
    geometry.cpp

HEADERS += \
    Polygon_splitter.h \
    geometry_global.h \
    geometry.h

# Default rules for deployment.
unix {
    target.path = /usr/lib
}
!isEmpty(target.path): INSTALLS += target
