# Read TIM2 and PIM2 images

Argumentos :
  a0
  a1 : puntero a fuente?

Stack :
  0 : s0
  4 : s1
  8 : ra

08837E5C read image :

    addiu  sp,sp,-0x10           # Prepara el stack para guardar
    sw  s0,0x0(sp)               # Guarda registro s0
    move  s0,a0                  # s0 = $0
    li  a0,0                     # a0 = false (para el caso de salir)
    sw  s1,0x4(sp)               # Guarda registro s1
    move  s1,a1                  # s1 = fontPtr
    sw  ra,0x8(sp)               # Guarda registro ra
    sw  a1,0x4(s0)               # [$0+4] = fontPtr
    beq  a1,zero,salida          # Si no hay fuente
      + sw  a1,0x8(s0)           # [$0+8] = fontPtr
    li  a1,0x0890B970            # a1 = 0x0890B970 -> "PIM2"
    move  a0,s1                  # a0 = fontPtr
    jal  cmpbytes                # Compara que sea "PIM2"
      + li  a2,0x4               # a2 = bytes a comparar
    bne  v0,zero,comprueba_tim2  # Si NO lo es :'(
      + li  v0,0x1               # Error
    sw  v0,0x48(s0)              # [$0+0x48] = 1
    lw  v1,0x8(s0)               # v1 = fontPtr

esImagen :

    li  a0,0x1                   # a0 = 1
    lhu  v0,0x6(v1)              # v0 = [fontPtr+6] (16 bits)
    beq  v0,a0,esVersion1        # Si es 1
      + lui  v0,0x892            # v0 = 0x08920000
    sw  a0,0x53A8(v0)            # [0x089253A8] = 1

esVersion1 :

    sw  zero,0x44(s0)            # [$0+0x44] = 0
    lbu  v0,0x5(v1)              # v0 = [fontPtr+5] (8 bits)
    bne  v0,zero,leeDatos        # Si no es 0
      + addiu  v0,s1,0x80        # v0 = fontPtr + 0x80
    addiu  v0,s1,0x10            # v0 = fontPtr + 0x10

leeDatos :

    sw  v0,0xC(s0)               # [$0+0xC] = dataPtr
    lw  a2,0xC(s0)               # a2 = dataPtr
    addiu  a0,a2,0x30            # a0 = dataPtr + 0x30
# beq  a0,zero,noEsFormato42     # COMO COÑO SE VA A DAR ESTO
    sw  a0,0x34(s0)              # [$0+0x34] = data2Ptr
    lb  v1,0x30(a2)              # v1 = [data2Ptr] (8 bits)
    li  v0,0x42                  # v0 = 0x42
    beql  v1,v0,esFormato42      # Si [data2Ptr] == 0x42
      - lb  v1,0x1(a0)           # v1 = [data2Ptr+1] (8 bits)

noEsFormato42 :

    lhu  v0,0xC(a2)              # v0 = [dataPtr+0xC] (16 bits) IMG_PTR

procesaCabecera :

    lhu  a0,0x16(a2)             # a0 = [dataPtr+0x16] (16 bits) ALTO
    lhu  v1,0x14(a2)             # v1 = [dataPtr+0x14] (16 bits) ANCHO
    addu  a3,a2,v0               # a3 = dataPtr + v0
    sw  a0,0x1C(s0)              # [$0+0x1C] = alto
    li  a1,0x0                   # a1 = 0
    li  a0,0x3F                  # a0 = 0x3F
    sw  v1,0x18(s0)              # [$0+0x18] = ancho
    sw  a3,0x14(s0)              # [$0+0x14] = imgPtr
    lw  v0,0x18(a2)              # v0 = [dataPtr+0x18] (32 bits)
    srl  v0,v0,0x14              # v0 >>= 0x14
    and  v0,v0,a0                # v0 &= 0x3F (bits 20-26)
    sw  v0,0x20(s0)              # [$0+0x20] = v0
    lbu  a0,0x12(a2)             # a0 = [dataPtr+0x12]
    andi  a0,a0,0x3F             # a0 &= 0x3F (bits 0-6)
    beql  a0,zero,salidaA0Es1    # Si a0 es 0
      - sw  zero,0x24(s0)        # [$0+0x24] = 0
    lw  a1,0x4(a2)               # a1 = [dataPtr+0x4]
    lw  v0,0x8(a2)               # v0 = [dataPtr+0x8] DATA SIZE
    addiu  v1,a1,0xFF            # v1 = a1 + 0xFF
    srl  a1,v1,0x8               # a1 = v1 >> 8 (+255/256) CASE NUMBER
    addu  v0,a3,v0               # v0 += imgPtr END DATA PTR
    sltiu  v1,a1,0x11            # Comprueba que no hay más de 16 casos
    beq  v1,zero,case0           # Si hay más casos de la cuenta...
    sw  v0,0x24(s0)              # [$0+0x24] = v0
    lui  v1,0x891                # Get case table
    sll  v0,a1,0x2               # Get table index
    addiu  v1,v1,-0x4680         # ... case table
    addu  v0,v0,v1               # Get function case
    lw  a0,0x0(v0)               # a0 = [v0] (32 bits)
    jr  a0
      + nop

case 1 :
    li  v0,0x8                   # v0 = 0x08

guardaAlgo :

    sw  v0,0x2C(s0)              # [$0+0x2C] = v0
    sw  v0,0x28(s0)              # [$0+0x28] = v0

leeGuarda1C :

    lw  v0,0x1C(a2)              # v0 = [dataPtr+0x1C] (32 bits)
    li  a1,0x0                   # a1 = 0
    li  a0,0xF                   # a0 = 0xF
    srl  v0,v0,0x13              # v0 >= 0x13
    and  v0,v0,a0                # v0 &= 0xF (bits 19-28)
    sw  v0,0x30(s0)              # [$0+0x30] = v0

salidaA0Es1 :

    li  a0,0x1                   # a0 = 1

salida :

    lw  ra,0x8(sp)               # Recupera el registro ra
    lw  s1,0x4(sp)               # Recupera el registro s1
    lw  s0,0x0(sp)               # Recupera el registro s0
    move  v0,a0                  # Devuelve el resultado
    jr  ra                       # Sale
      + addiu  sp,sp,0x10        # Devuelve el stack al estado original

case 2 :
    li  v0,0x10
    li  v1,0x8
    sw  v0,0x28(s0)

guarda2CLee1C :

    j  leeGuarda1C
     + sw  v1,0x2C(s0)

case 4 :
    j  guardaAlgo
     + li  v0,0x10

case 8 :
    li  v0,0x20
    sw  v0,0x28(s0)
    j  guarda2CLee1C
      + li  v1,0x10

case 0 | 3 | 5 | 6 | 7 | 9 | 10 | 11 | 12 | 13 | 14 | 15 :
    li  v0,0x40
    sw  v0,0x28(s0)
    j  leeGuarda1C
     + sw  a1,0x2C(s0)

case 16 :
    j  guardaAlgo
     + li  v0,0x20

esFormato42 :

    li  v0,0x55
    bnel  v1,v0,procesaCabecera
      - lhu  v0,0xC(a2)
    lb  v1,0x2(a0)
    li  v0,0x56
    bnel  v1,v0,procesaCabecera
     - lhu  v0,0xC(a2)
    lw  v1,0x34(a2)
    addiu  v0,a2,0x38
    sw  v0,0x3C(s0)
    j  noEsFormato42
     + sw  v1,0x38(s0)

comprueba_tim2 :

    sw  zero,0x48(s0)            # [$0+0x48] = 0
    li  a1,0x0890B978            # a1 = 0x0890B970 -> "TIM2"
    lw  a0,0x8(s0)               # a0 = fontPtr
    jal  cmpbytes                # Compara que sea "TIM2"
      + li  a2,0x4               # ... sus 4 primeros bytes
    beql  v0,zero,esImagen       # Si es "TIM2"
      - lw  v1,0x8(s0)           # v1 = fontPtr
    li  v1,0x1
    lui  v0,0x892
    li  a0,0
    sw  v1,0x53A8(v0)
    j  salida
      + sw  zero,0x4(s0)
