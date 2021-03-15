<?php
//$dbConnection = new PDO('mysql:dbname=angrpjug_ListenIn;host=localhost;charset=utf8', 'angrpjug_li', 'listenin2016');
$dbConnection = new PDO('mysql:dbname=ucweyon_listenin_rct;host=mysql-server.ucl.ac.uk;charset=utf8', 'ucweyon_user', 'ucweyon');
$dbConnection->setAttribute(PDO::ATTR_EMULATE_PREPARES, false);
$dbConnection->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
$dbConnection->setAttribute(PDO::MYSQL_ATTR_MAX_BUFFER_SIZE, 1024*1024*15);  // increase buffer size from the default 1M to 5M
// Set timezone
 date_default_timezone_set("Europe/London");
?>
