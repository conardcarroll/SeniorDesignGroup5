<?php
//echo "start";
$u = $_GET['u'];
$p = $_GET['p'];
$database="creechky"; 
$host="ucfsh.ucfilespace.uc.edu"; 
$username="creechky"; 
$password="BadWolf11";

// Create connection
$con=mysqli_connect($host,$username,$password,$database);

// Check connection

if (mysqli_connect_errno()) 
{
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}

$FamilyIDresult = mysqli_query($con,"SELECT * FROM wedding WHERE User_Name = '".$u."' and password = '".$p."'");

$FamilyID = -1;

while($row = mysqli_fetch_array($FamilyIDresult)) 
{
$FamilyID = $row['FamilyID_Number'];
//echo $FamilyID;
readfile("user.html");
}
if ($FamilyID == -1)
 {
    echo "</br></br></br></br><h2> Invalid User/Password </h2>";
	readfile("Login.html");

 }


//echo "test";

if ($FamilyID > -1)
{  
$result = mysqli_query($con,"SELECT * FROM wedding WHERE familyid_number = '{$FamilyID}'");
$guestNumber = 0;

echo "<div id='forms'>";

echo "";


while($row = mysqli_fetch_array($result)) 

{
  if($guestNumber == 0)
  {
	echo "<form name='{$row['First_Name']}' guestnumber='{$row['UId']}'>";
    echo "<h4>{$row['First_Name']}</h4></br>";
    echo "<div class ='info'> </br></br>Email Address:<input type='text'id='Email' name='Email'  value='{$row['Email']}'>   </div>";
  }
  if($guestNumber > 0)
  {
      //echo "test{$guestNumber}";
      echo "<form name='Guest {$guestNumber}' guestnumber='{$row['UId']}'>";
      echo "<h4>Guest {$guestNumber}</h4>";
      
  }
  
  $noChoice = "";
  $attending = "";
  $notAttending = "";
  if($row['Attending'] == "NULL") {
 	  $noChoice = " selected";
  } else if($row['Attending'] == 1) {
 	  $attending = " selected";
  } else {
 	  $notAttending = " selected";
  }
  
  $darray = array("","","", "", "");
  $darray[$row['Dinner_Number'] - 1] = " checked ";
  
  $sarray = array("","");
  $sarray[$row['Salad'] - 1] = " checked ";
  
  echo "<div class ='info'>First Name:"."<input type='text' id='first' name='firstName'  value='{$row['First_Name']}'>   ";
  echo "Last Name:"."<input type='text' id='last' name='lastName'  value='{$row['Last_Name']}'>   ";
  echo "Attending:"."<select id='attending{$row['UId']}'><option value='NULL' {$noChoice}>Select</option><option value='1' {$attending}>YES</option><option value='0' {$notAttending}>NO</option></select>   ";

  echo "</div>";
  echo "<h6>Dinner Options:</br></br><h7>Salad Selections:</br>";
  
  echo "<h8><input type='radio' name='s{$row['UId']}' value='1' {$sarray[0]}><b>Classic Caesar Salad</b></br>
  <h9>Crisp Romaine Lettuce Tossed with Parmesan Chesse, Crutons and Classic Caesar Dressing";
  echo "</h8></h9></br>";
  echo "<h8><input type='radio' name='s{$row['UId']}' value='2' {$sarray[1]}><b>Spinach Salad</b></br>
  <h9>Fresh Spinach Leaves with Sliced Mushrooms, Mandarin Orange Segments, Red Onion & Diced Tomatoes with Warm Bacon Dressing";
  echo "</h7></h8></h9></br></br>";
  
   echo "<h7>Adult Entree Selections:</br>";

  echo "<h8><input type='radio' name='e{$row['UId']}' value='1' {$darray[0]}><b>Smoked Gouda and Prosciutto Chicken</b></br>
  <h9>Tender Chicken Breast Filled with Smoked Gouda and Prosciutto Served in a Light Basil Cream Sauce";
  echo "</h8></h9></br>";

  echo "<h8><input type='radio' name='e{$row['UId']}' value='2' {$darray[1]}><b>Frenched Pork Chop</b></br>
  <h9>10 oz. Grilled Pork Chop Topped with Brandied Granny Smith Apples & Sweet Basil";
  echo "</h7></h8></h9></br></br>";
  
     echo "<h7>Kids Under 10 Years Old Entree Selections:</br>";
  
  echo "<h8><input type='radio' name='e{$row['UId']}' value='3' {$darray[2]}><b>Chicken Fingers and French Fries</b></h8></br>";
  echo "<h8><input type='radio' name='e{$row['UId']}' value='4' {$darray[3]}><b>Noble Roman's Pepperoni or Cheese Pizza (7-inch)</b></h8></br>";
  echo "<h7>Vegetarian Choice:</br>";
  echo "<h8><input type='radio' name='e{$row['UId']}' value='5' {$darray[4]}><b>Vegetarian Choice</b>";
  echo "</h6></h7></h8><br><br>";

  echo "</form>";
  
  echo "";

 
  $guestNumber = $guestNumber + 1;
  
}
readfile("userFooter.html");
}


mysqli_close($con);
?>