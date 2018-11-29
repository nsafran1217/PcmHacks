PREFIX = freescale-coldfire-2011.09/bin/m68k-linux-gnu-

AS = $(PREFIX)as
CC = $(PREFIX)gcc
LD = $(PREFIX)ld
OBJDUMP = $(PREFIX)objdump

ASFLAGS =
CCFLAGS = -mcpu32 -c -fomit-frame-pointer -std=gnu99
LDFLAGS = --oformat binary -T link.lds
DUMPFLAGS = -D -b binary -mm68k:cpu32

all: micro-kernel.o micro-kernel.bin

%.o: %.c
	$(CC) $(CCFLAGS) $< -o $@

%.bin: %.o
	$(LD)  $(LDFLAGS) $< -o $@
	$(OBJDUMP) $(DUMPFLAGS) $@ > $@.disassembly

clean:
	rm -f *.bin *.o *.disassembly
