<?php
require 'config/config.php'; 

$id_user = $_POST["id_user"];
$period = $_POST["period"];
$date_subscribed = date('Y-m-d H:i:s', time());

$current_date = date('H:i:s m-d-Y', time());
$date_subscription = mktime($current_date);
//ADD A MONTH
$date_subscription += 60*60*24*(30 * $period);
$date_subscription = date('Y-m-d H:i:s', $date_subscription);

$preparedStatementVersion = dbConnection::get()->prepare('UPDATE user SET date_subscription = :date_subscription, date_subscribed = :date_subscribed WHERE id_user = :id_user LIMIT 1');
$preparedStatementVersion->execute(array('id_user' => $id_user, 'date_subscription' => $date_subscription, 'date_subscribed' => $date_subscribed));

echo date_format(date_create($date_subscription), 'Y-m-d');

?>
 