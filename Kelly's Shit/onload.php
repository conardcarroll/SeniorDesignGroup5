<?php
$database="creechky"; 
$host="ucfsh.ucfilespace.uc.edu"; 
$username="creechky"; 
$password="BadWolf11";

 $con=mysqli_connect($host,$username,$password,$database);

// Check connection
if (mysqli_connect_errno()) {
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}
else    
{
echo readfile("Login.html");
}
?>

