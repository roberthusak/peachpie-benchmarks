<?php
/**
 * The base configuration for WordPress
 *
 * The wp-config.php creation script uses this file during the
 * installation. You don't have to use the web site, you can
 * copy this file to "wp-config.php" and fill in the values.
 *
 * This file contains the following configurations:
 *
 * * MySQL settings
 * * Secret keys
 * * Database table prefix
 * * ABSPATH
 *
 * @link https://wordpress.org/support/article/editing-wp-config-php/
 *
 * @package WordPress
 */

// ** MySQL settings - You can get this info from your web host ** //
/** The name of the database for WordPress */
define( 'DB_NAME', 'db' );

/** MySQL database username */
define( 'DB_USER', 'root' );

/** MySQL database password */
define( 'DB_PASSWORD', 'root' );

/** MySQL hostname */
define( 'DB_HOST', 'localhost' );

/** Database Charset to use in creating database tables. */
define( 'DB_CHARSET', 'utf8mb4' );

/** The Database Collate type. Don't change this if in doubt. */
define( 'DB_COLLATE', '' );

/**#@+
 * Authentication Unique Keys and Salts.
 *
 * Change these to different unique phrases!
 * You can generate these using the {@link https://api.wordpress.org/secret-key/1.1/salt/ WordPress.org secret-key service}
 * You can change these at any point in time to invalidate all existing cookies. This will force all users to have to log in again.
 *
 * @since 2.6.0
 */
define( 'AUTH_KEY',         'kH}5xAj%]@wk/:d4XH}ng6&SSzy7R 6cLQn|fYZ&duSEtEq|y:_9O@3p{BkVE7Gp' );
define( 'SECURE_AUTH_KEY',  'vw@&axX*~dhJQ8~<==D:NK;Az5,Ym2Q%Y+%=cG]?ipuuTtqBGo%7-Q~Y6J075HiV' );
define( 'LOGGED_IN_KEY',    'GIT%UTx^qI%N@bN&{uS@K-wUSQj0,~z`k]9/Uxfre^B8#yn#c!X*6Kla@w8(:}^u' );
define( 'NONCE_KEY',        '[8?!A*/F xnc<p&Q@5sR`C/Fwp$3,_RD=VnA|u<9S=6c;Gy*;KIZk1X[|6=j03*Z' );
define( 'AUTH_SALT',        '-:bKT;.~X<O_dGPO`D*7`Enjg1>g{eha/vRx;DYrSX,>KtG0=}TKZ;[]XfA(YFVG' );
define( 'SECURE_AUTH_SALT', '<zE[R]0c7;AfJ9NpXArV+UeF!fW64S<4 @A&TJI81N_CDkCg;i}EHlAy!AHsEp#B' );
define( 'LOGGED_IN_SALT',   'nwK;m`9n/N/n967E{L]S$KdX: uuFCBc5ZpM`K<q,O@E^j#A;,R,oo1U$*(q+1I}' );
define( 'NONCE_SALT',       ';e78,r7u>pFgz@^,>Pxf2,% ]kO[~yA6Ro-Je*ty^$uKD=Nn00W]&AbZ==~^AGzF' );

/**#@-*/

/**
 * WordPress Database Table prefix.
 *
 * You can have multiple installations in one database if you give each
 * a unique prefix. Only numbers, letters, and underscores please!
 */
$table_prefix = 'wp_';

/**
 * For developers: WordPress debugging mode.
 *
 * Change this to true to enable the display of notices during development.
 * It is strongly recommended that plugin and theme developers use WP_DEBUG
 * in their development environments.
 *
 * For information on other constants that can be used for debugging,
 * visit the documentation.
 *
 * @link https://wordpress.org/support/article/debugging-in-wordpress/
 */
define( 'WP_DEBUG', false );

/* That's all, stop editing! Happy publishing. */

/** Absolute path to the WordPress directory. */
if ( ! defined( 'ABSPATH' ) ) {
        define( 'ABSPATH', __DIR__ . '/' );
}

/** Sets up WordPress vars and included files. */
require_once ABSPATH . 'wp-settings.php';
