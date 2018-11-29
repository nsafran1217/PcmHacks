CodeWarrior USB Device Driver Manual Installation:
--------------------------------------------------

Supported Devices:

  CodeWarrior USB TAP

Supported Operating Systems*:

  Red Hat Linux 8.0, 9.0, Fedora Core 3, Fedora Core 4
  Mandrake Linux 10.0, SuSE 10.1

  * Limited testing has been performed on the distributions listed above.
    In general, the USB TAP should be usable on any distribution which uses
    a 2.4 or 2.6 kernel, but CCS may need to be run as root if the
    distribution's hotplug scripts differ significantly from the tested
    distributions.

Driver Type:

  libusb Version 0.1.12

Installation Instructions:

  Please note that the following procedures assume that the user has 
  the Administrator privileges.

  There are two different systems for handling USB devices on linux:
  devfs/hotplug, and udev.

  If you have a directory named /etc/udev/rules.d, then your distribution uses
  udev, and you should perform the following steps:

    1.  Install udev rules

        Copy 00-usbtap.rules to /etc/udev/rules.d

    2.  Activate new udev rules

        Run the command 'udevcontrol reload_rules'

    3.  Plug in USB TAPs

        NOTE: If you already had your USB TAPs plugged in prior to performing
        the above modifications, you will need to unplug them and plug them in
        again in order for the changes to take effect.

  If you have a directory named /etc/hotplug, then your distribution uses
  devfs/hotplug, and you should perform the following steps:

    1.  Install usermap data

        On Redhat systems, you should prepend the contents of this file to
        /etc/hotplug/usb.usermap.

        On Mandrake systems, copy it to /usr/lib/hotplug/usbtap/usb.usermap,
        make sure it is readable by root, but *not* be executable, then run
        update-usb.usermap.


    2.  Install hotplug script

        Copy usbtap to /etc/hotplug/usb make sure it is executable by root.

    3.  Plug in USB TAPs

        NOTE: If you already had your USB TAPs plugged in prior to performing
        the above modifications, you will need to unplug them and plug them in
        again in order for the changes to take effect.

  If you are unsure, or both directories are present on your system, then you
  can try following both sets of instructions.
