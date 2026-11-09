Metodo: GenerateJWT(){}
Objetivo: Crear un token JWT firmado que contiene información del usuario (ID, grupo, rol) y que expira en 10 minutos.

Metodo: TokenValidate(){}
Objetivo: Verifica sin un token jwt es legitimo, no ha expirado y fue firmado correctamente con la clave secreta de la aplicación.
