#!/bin/bash
curl -o wordpress.tar.gz https://wordpress.org/wordpress-5.4.1.tar.gz
tar --strip-components=1 -xf wordpress.tar.gz --directory $1
cp wp-config.php $1

# Peachpie can't compile them (missing base classes)
rm -rf $1/wp-includes/class-json.php
rm -rf $1/wp-content/plugins/akismet
