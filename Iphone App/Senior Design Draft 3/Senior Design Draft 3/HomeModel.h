//
//  HomeModel.h
//  Senior Design Draft 3
//
//  Created by Joshua Smith on 2/22/15.
//  Copyright (c) 2015 Joshua Smith. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol HomeModelProtocol <NSObject>

- (void)itemsDownloaded:(NSArray *)items;

@end

@interface HomeModel : NSObject <NSURLConnectionDataDelegate>

@property (nonatomic, weak) id<HomeModelProtocol> delegate;

- (void)downloadItems;

@end
