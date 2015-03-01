//
//  ViewController.h
//  Senior Design Draft 3
//
//  Created by Joshua Smith on 2/22/15.
//  Copyright (c) 2015 Joshua Smith. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "HomeModel.h"

@interface ViewController : UIViewController <UITableViewDataSource, UITableViewDelegate, HomeModelProtocol>

@property (weak, nonatomic) IBOutlet UITableView *listTableView;

@end

