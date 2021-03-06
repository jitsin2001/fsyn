[IFUNDEF] 2PI
include constants.fs
[ENDIF]

\ setup timestamp; unit will be samples.
variable sample_clock
0 sample_clock !

\ setup channels
variable lchan 0.0e lchan f!
variable rchan 0.0e rchan f!

\ sample clock (timestamp) and channel routines here
: t sample_clock @ ;
: lcget lchan f@ ;
: lcset lchan f! ;
: rcget rchan f@ ;
: rcset rchan f! ;
: clear_channels
  0.0e lcset 0.0e rcset ;
: t+ 1 sample_clock +!  \ advance the clock
  clear_channels 
  reset-phasor-count ;

\ sending out mono or stereo
: scale_output
  BIT_DEPTH f* f>s ;

: panmix ( sig pan -- ) \ will send out to global L and R channels 
  fabs fsqrt fdup 1e fswap f- frot fdup frot 
  f* lcget f+ lcset
  f* rcget f+ rcset ;

: sendout-32 ( fval -- )  
  scale_output dup dup dup
  0xff and emit           \ send 1st byte (least-significant)
  8 >> 0xff and emit      \ send 2nd byte (16-bit most-significant)
  16 >> 0xff and emit     \ send 3rd byte (24-bit most-significant) 
  24 >> 0xff and emit ;   \ send 4th byte (32-bit most-significant)

: sendout-16 ( fval -- )
  scale_output dup
  0xff and emit
  8 >> 0xff and emit ;
 
: stereo_out ( sig sig -- )
  lcget sendout-32 rcget sendout-32 ;
