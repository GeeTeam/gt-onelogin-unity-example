//
//  OneLoginUnityPlugin.m
//  OneLoginUnityPlugin
//
//  Created by noctis on 2020/6/19.
//  Copyright © 2020 com.geetest. All rights reserved.
//

#import "OneLoginUnityPlugin.h"
#import <OneLoginSDK/OneLoginSDK.h>

#define UnityPlugin ([OneLoginUnityPlugin sharedInstance])

#if defined(__cplusplus)
extern "C"{
#endif
    // 桥接方法，Unity 中调用
    extern char* getCurrentCarrier();
    extern void setProtocolCheckState(bool isChecked);
    extern void deletePreResultCache();

    extern void registerCallback(const char *objName, const char *requestTokenCallbackName, const char *getPhoneCallbackName);
    extern void registerWihtAppID(const char *appId);
    extern void enterAuthController(const char *configs, char **widgets);
    extern void dismissAuthViewController();
    extern void setLogEnabled(bool enabled);
    extern void renewPreGetToken();
    extern bool isPreGetTokenResultValidate();
    extern void setRequestTimeout(double timeout);
    extern char* sdkVersion();

    extern void registerOnepassCallback(const char *objName, const char *verifyPhoneCallbackName, const char *validatePhoneCallbackName);
    extern void initOnePass(const char *customID, double timeout);
    extern void verifyPhoneNumber(const char *phoneNumber);
    extern void validateOnePassAccessCode(const char *accessCode, const char *customId, const char *processId, const char *phone, const char *operatorType);
                                 
    extern void validateToken(const char *token, const char *appID, const char *processID, const char *authcode);
    extern void showAlertMessage(const char *message);
#if defined(__cplusplus)
}
#endif

@interface GTButton : UIButton

@property (nonatomic, copy) NSString *buttonAction;

@end

@implementation GTButton

@end

@interface OneLoginUnityPlugin () <GOPManagerDelegate>

@property (nonatomic, copy) NSString *objName;
@property (nonatomic, copy) NSString *requestTokenCallbackName;
@property (nonatomic, copy) NSString *getPhoneCallbackName;

@property (nonatomic, assign) NSTimeInterval timeInterval;

@property (nonatomic, copy) NSString *verifyPhoneCallbackName;
@property (nonatomic, copy) NSString *validatePhoneCallbackName;

@property (nonatomic, strong) GOPManager *gopManager;

@end

@implementation OneLoginUnityPlugin

// MARK: Init

+ (instancetype)sharedInstance {
    static OneLoginUnityPlugin *up = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        up = [[OneLoginUnityPlugin alloc] init];
    });
    return up;
}

// MARK: OneLogin Methods

- (NSString *) getCurrentCarrier {
    OLNetworkInfo *networkInfo = [OneLoginPro currentNetworkInfo];
    return networkInfo.carrierName;
}
- (void) setProtocolCheckState:(BOOL)isChecked {
    [OneLoginPro setProtocolCheckState:isChecked];
}

- (void) deletePreResultCache {
    [OneLoginPro deletePreResultCache];
}

- (void)registerCallbackWithObjName:(NSString *)objName requestTokenCallbackName:(NSString *)requestTokenCallbackName getPhoneCallbackName:(NSString *)getPhoneCallbackName {
    NSLog(@"============ register callback ==============");
    NSLog(@"============ objName:%@ ==============",objName);
    NSLog(@"============ requestTokenCallbackName:%@ ==============",requestTokenCallbackName);
    NSLog(@"============ getPhoneCallbackName:%@ ==============",getPhoneCallbackName);
    self.objName = objName;
    self.requestTokenCallbackName = requestTokenCallbackName;
    self.getPhoneCallbackName = getPhoneCallbackName;
}

- (void)registerWihtAppID:(NSString *)appId {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    NSLog(@"============ registerWithAppId ==============");
    [OneLoginPro registerWithAppID:appId];
}

- (void)enterAuthViewController:(NSString *)configs widgets:(NSArray *)widgets {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    NSLog(@"============ enterAuthViewController ==============\r\n");
    NSLog(@"============\r\n configs: %@ \r\n==============", configs);
    
    OLAuthViewModel *viewModel = [[OLAuthViewModel alloc] init];
    NSError *jsonError = nil;
    NSDictionary *viewModelDict = [NSJSONSerialization JSONObjectWithData:[configs dataUsingEncoding:NSUTF8StringEncoding] options:(NSJSONReadingOptions)0 error:&jsonError];
    if (nil == jsonError && [viewModelDict isKindOfClass:[NSDictionary class]] && viewModelDict.count > 0) {
        // *************** languageType *************** //
        if (viewModelDict[@"languageType"]) {
            viewModel.languageType = (OLLanguageType)[viewModelDict[@"languageType"] integerValue];
        }
        
        // *************** statusBarStyle *************** //
        if (viewModelDict[@"statusBarStyle"]) {
            viewModel.statusBarStyle = (UIStatusBarStyle)[viewModelDict[@"statusBarStyle"] integerValue];
        }
        
        // *************** naviTitle *************** //
        if (viewModelDict[@"naviTitle"]) {
            NSString *naviTitleString = [NSString stringWithFormat:@"%@", viewModelDict[@"naviTitle"]];
            NSMutableAttributedString *naviTitle = [[NSMutableAttributedString alloc] initWithString:naviTitleString];
            if (viewModelDict[@"naviTitleColor"]) {
                [naviTitle addAttributes:@{NSForegroundColorAttributeName: [self colorFromHexString:viewModelDict[@"naviTitleColor"]] ?: UIColor.blackColor} range:NSMakeRange(0, naviTitleString.length)];
            }
            if ([self fontFromString:viewModelDict[@"naviTitleFont"]]) {
                [naviTitle addAttributes:@{NSFontAttributeName: [self fontFromString:viewModelDict[@"naviTitleFont"]]} range:NSMakeRange(0, naviTitleString.length)];
            }
            viewModel.naviTitle = naviTitle.copy;
        }
        
        if ([self colorFromHexString:viewModelDict[@"naviBgColor"]]) {
            viewModel.naviBgColor = [self colorFromHexString:viewModelDict[@"naviBgColor"]];
        }
        
        UIImage *naviBackImage = [self imageWithName:viewModelDict[@"naviBackImage"]];
        if (nil != naviBackImage) {
            viewModel.naviBackImage = naviBackImage;
        }
        
        if (viewModelDict[@"naviHidden"]) {
            viewModel.naviHidden = [viewModelDict[@"naviHidden"] boolValue];
        }
        
        if (viewModelDict[@"backButtonRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"backButtonRect"]]]) {
                viewModel.backButtonRect = [self rectFromString:viewModelDict[@"backButtonRect"]];
            }
        }
        
        if (viewModelDict[@"backButtonHidden"]) {
            viewModel.backButtonHidden = [viewModelDict[@"backButtonHidden"] boolValue];
        }
        
        // *************** appLogo *************** //
        UIImage *appLogo = [self imageWithName:viewModelDict[@"appLogo"]];
        if (nil != appLogo) {
            viewModel.appLogo = appLogo;
        }
        
        if (viewModelDict[@"logoRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"logoRect"]]]) {
                viewModel.logoRect = [self rectFromString:viewModelDict[@"logoRect"]];
            }
        }
        
        if (viewModelDict[@"logoHidden"]) {
            viewModel.logoHidden = [viewModelDict[@"logoHidden"] boolValue];
        }
        
        if (viewModelDict[@"logoCornerRadius"]) {
            viewModel.logoCornerRadius = [viewModelDict[@"logoCornerRadius"] doubleValue];
        }
        
        // *************** phoneNum *************** //
        if ([self colorFromHexString:viewModelDict[@"phoneNumColor"]]) {
            viewModel.phoneNumColor = [self colorFromHexString:viewModelDict[@"phoneNumColor"]];
        }
        
        if ([self fontFromString:viewModelDict[@"phoneNumFont"]]) {
            viewModel.phoneNumFont = [self fontFromString:viewModelDict[@"phoneNumFont"]];
        }
        
        if (viewModelDict[@"phoneNumRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"phoneNumRect"]]]) {
                viewModel.phoneNumRect = [self rectFromString:viewModelDict[@"phoneNumRect"]];
            }
        }
        
        // *************** Switch Button *************** //
        if (viewModelDict[@"switchButtonText"]) {
            NSString *switchButtonText = [NSString stringWithFormat:@"%@", viewModelDict[@"switchButtonText"]];
            if ([self isValidString:switchButtonText]) {
                viewModel.switchButtonText = switchButtonText;
            }
        }
        
        if ([self colorFromHexString:viewModelDict[@"switchButtonColor"]]) {
            viewModel.switchButtonColor = [self colorFromHexString:viewModelDict[@"switchButtonColor"]];
        }
        
        if ([self colorFromHexString:viewModelDict[@"switchButtonBackgroundColor"]]) {
            viewModel.switchButtonBackgroundColor = [self colorFromHexString:viewModelDict[@"switchButtonBackgroundColor"]];
        }
        
        if ([self fontFromString:viewModelDict[@"switchButtonFont"]]) {
            viewModel.switchButtonFont = [self fontFromString:viewModelDict[@"switchButtonFont"]];
        }
        
        if (viewModelDict[@"switchButtonRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"switchButtonRect"]]]) {
                viewModel.switchButtonRect = [self rectFromString:viewModelDict[@"switchButtonRect"]];
            }
        }
        
        if (viewModelDict[@"switchButtonHidden"]) {
            viewModel.switchButtonHidden = [viewModelDict[@"switchButtonHidden"] boolValue];
        }
        
        // *************** Auth Button *************** //
        if (viewModelDict[@"authButtonImages"]) {
            NSArray *imageArray = viewModelDict[@"authButtonImages"];
            if (imageArray.count >= 3) {
                UIImage *image0 = [self imageWithName:imageArray[0]];
                UIImage *image1 = [self imageWithName:imageArray[1]];
                UIImage *image2 = [self imageWithName:imageArray[2]];
                if (image0 && image1 && image2) {
                    viewModel.authButtonImages = @[image0, image1, image2];
                }
            }
        }
        
        if (viewModelDict[@"authButtonTitle"]) {
            NSString *authButtonTitleString = [NSString stringWithFormat:@"%@", viewModelDict[@"authButtonTitle"]];
            if ([self isValidString:authButtonTitleString]) {
                NSMutableAttributedString *authButtonTitle = [[NSMutableAttributedString alloc] initWithString:authButtonTitleString];
                if (viewModelDict[@"authButtonTitleColor"]) {
                    [authButtonTitle addAttributes:@{NSForegroundColorAttributeName: [self colorFromHexString:viewModelDict[@"authButtonTitleColor"]] ?: UIColor.blackColor} range:NSMakeRange(0, authButtonTitleString.length)];
                }
                if ([self fontFromString:viewModelDict[@"authButtonTitleFont"]]) {
                    [authButtonTitle addAttributes:@{NSFontAttributeName: [self fontFromString:viewModelDict[@"authButtonTitleFont"]]} range:NSMakeRange(0, authButtonTitleString.length)];
                }
                viewModel.authButtonTitle = authButtonTitle.copy;
            }
        }
        
        if (viewModelDict[@"authButtonRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"authButtonRect"]]]) {
                viewModel.authButtonRect = [self rectFromString:viewModelDict[@"authButtonRect"]];
            }
        }
        
        if (viewModelDict[@"authButtonCornerRadius"]) {
            viewModel.authButtonCornerRadius = [viewModelDict[@"authButtonCornerRadius"] doubleValue];
        }
        
        // *************** slogan *************** //
        if (viewModelDict[@"sloganRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"sloganRect"]]]) {
                viewModel.sloganRect = [self rectFromString:viewModelDict[@"sloganRect"]];
            }
        }
        
        if ([self colorFromHexString:viewModelDict[@"sloganTextColor"]]) {
            viewModel.sloganTextColor = [self colorFromHexString:viewModelDict[@"sloganTextColor"]];
        }
        
        if ([self fontFromString:viewModelDict[@"sloganTextFont"]]) {
            viewModel.sloganTextFont = [self fontFromString:viewModelDict[@"sloganTextFont"]];
        }
        
        // *************** Privacy Terms *************** //
        if (viewModelDict[@"defaultCheckBoxState"]) {
            viewModel.defaultCheckBoxState = [viewModelDict[@"defaultCheckBoxState"] boolValue];
        }
        
        if ([self imageWithName:viewModelDict[@"checkedImage"]]) {
            viewModel.checkedImage = [self imageWithName:viewModelDict[@"checkedImage"]];
        }
        
        if ([self imageWithName:viewModelDict[@"uncheckedImage"]]) {
            viewModel.uncheckedImage = [self imageWithName:viewModelDict[@"uncheckedImage"]];
        }
        
        if (viewModelDict[@"checkBoxRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"checkBoxRect"]]]) {
                viewModel.checkBoxRect = [self rectFromString:viewModelDict[@"checkBoxRect"]];
            }
        }
        
        NSMutableDictionary *privacyTermsAttributes = [NSMutableDictionary dictionary];
        if (viewModelDict[@"privacyTermsColor"]) {
            UIColor *privacyTermsColor = [self colorFromHexString:viewModelDict[@"privacyTermsColor"]];
            if (privacyTermsColor) {
                [privacyTermsAttributes setValue:privacyTermsColor forKey:NSForegroundColorAttributeName];
            }
        }
        if ([self fontFromString:viewModelDict[@"privacyTermsFont"]]) {
            UIFont *privacyTermsFont = [self fontFromString:viewModelDict[@"privacyTermsFont"]];
            if (privacyTermsFont) {
                [privacyTermsAttributes setValue:privacyTermsFont forKey:NSFontAttributeName];
            }
        }
        if (privacyTermsAttributes.count > 0) {
            viewModel.privacyTermsAttributes = privacyTermsAttributes.copy;
        }
        
        if (viewModelDict[@"additionalPrivacyTerms"]) {
            NSArray *additionalPrivacyTerms = viewModelDict[@"additionalPrivacyTerms"];
            if (additionalPrivacyTerms.count > 0) {
                NSMutableArray<OLPrivacyTermItem *> *items = [NSMutableArray arrayWithCapacity:additionalPrivacyTerms.count];
                for (NSInteger i = 0; i + 2 < additionalPrivacyTerms.count; i += 3) {
                    OLPrivacyTermItem *item = [[OLPrivacyTermItem alloc] initWithTitle:additionalPrivacyTerms[i]
                                                                               linkURL:[NSURL URLWithString:additionalPrivacyTerms[i + 1]]
                                                                                 index:[additionalPrivacyTerms[i + 2] integerValue]];
                    [items addObject:item];
                }
                
                if (items.count > 0) {
                    viewModel.additionalPrivacyTerms = items.copy;
                }
            }
        }
        
        if ([self colorFromHexString:viewModelDict[@"termTextColor"]]) {
            viewModel.termTextColor = [self colorFromHexString:viewModelDict[@"termTextColor"]];
        }
        
        if (viewModelDict[@"termsRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"termsRect"]]]) {
                viewModel.termsRect = [self rectFromString:viewModelDict[@"termsRect"]];
            }
        }
        
        if (viewModelDict[@"auxiliaryPrivacyWords"]) {
            NSArray *auxiliaryPrivacyWords = viewModelDict[@"auxiliaryPrivacyWords"];
            if (4 == auxiliaryPrivacyWords.count) {
                viewModel.auxiliaryPrivacyWords = auxiliaryPrivacyWords;
            }
        }
        
        if (viewModelDict[@"termsAlignment"]) {
            viewModel.termsAlignment = (NSTextAlignment)[viewModelDict[@"termsAlignment"] integerValue];
        }
        
        // *************** Background *************** //
        if ([self colorFromHexString:viewModelDict[@"backgroundColor"]]) {
            viewModel.backgroundColor = [self colorFromHexString:viewModelDict[@"backgroundColor"]];
        }
        
        if (viewModelDict[@"backgroundImage"]) {
            viewModel.backgroundImage = [self imageWithName:viewModelDict[@"backgroundImage"]];
        }
        
        if (viewModelDict[@"landscapeBackgroundImage"]) {
            viewModel.landscapeBackgroundImage = [self imageWithName:viewModelDict[@"landscapeBackgroundImage"]];
        }
        
        // *************** Popup *************** //
        if (viewModelDict[@"isPopup"]) {
            viewModel.isPopup = [viewModelDict[@"isPopup"] boolValue];
        }
        
        if (viewModelDict[@"popupRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"popupRect"]]]) {
                viewModel.popupRect = [self rectFromString:viewModelDict[@"popupRect"]];
            }
        }
        
        if (viewModelDict[@"popupCornerRadius"]) {
            viewModel.popupCornerRadius = [viewModelDict[@"popupCornerRadius"] doubleValue];
        }
        
        if (viewModelDict[@"popupRectCorners"]) {
            NSArray *popupRectCorners = viewModelDict[@"popupRectCorners"];
            if (popupRectCorners.count > 0) {
                viewModel.popupRectCorners = popupRectCorners;
            }
        }
        
        if (viewModelDict[@"popupAnimationStyle"]) {
            viewModel.popupAnimationStyle = (OLAuthPopupAnimationStyle)[viewModelDict[@"popupAnimationStyle"] integerValue];
        }
        
        if ([self imageWithName:viewModelDict[@"closePopupImage"]]) {
            viewModel.closePopupImage = [self imageWithName:viewModelDict[@"closePopupImage"]];
        }
        
        if (viewModelDict[@"closePopupTopOffset"]) {
            viewModel.closePopupTopOffset = [viewModelDict[@"closePopupTopOffset"] isKindOfClass:[NSNumber class]] ? viewModelDict[@"closePopupTopOffset"] : [NSNumber numberWithDouble:[viewModelDict[@"closePopupTopOffset"] doubleValue]];
        }
        
        if (viewModelDict[@"closePopupRightOffset"]) {
            viewModel.closePopupRightOffset = [viewModelDict[@"closePopupRightOffset"] isKindOfClass:[NSNumber class]] ? viewModelDict[@"closePopupRightOffset"] : [NSNumber numberWithDouble:[viewModelDict[@"closePopupRightOffset"] doubleValue]];
        }
        
        if (viewModelDict[@"canClosePopupFromTapGesture"]) {
            viewModel.canClosePopupFromTapGesture = [viewModelDict[@"canClosePopupFromTapGesture"] boolValue];
        }
        
        // *************** WebController NavigationBar *************** //
        if (viewModelDict[@"webNaviTitle"]) {
            NSString *webNaviTitleString = [NSString stringWithFormat:@"%@", viewModelDict[@"webNaviTitle"]];
            NSMutableAttributedString *webNaviTitle = [[NSMutableAttributedString alloc] initWithString:webNaviTitleString];
            if (viewModelDict[@"webNaviTitleColor"]) {
                [webNaviTitle addAttributes:@{NSForegroundColorAttributeName: [self colorFromHexString:viewModelDict[@"webNaviTitleColor"]] ?: UIColor.blackColor} range:NSMakeRange(0, webNaviTitleString.length)];
            }
            if ([self fontFromString:viewModelDict[@"webNaviTitleFont"]]) {
                [webNaviTitle addAttributes:@{NSFontAttributeName: [self fontFromString:viewModelDict[@"webNaviTitleFont"]]} range:NSMakeRange(0, webNaviTitleString.length)];
            }
            viewModel.webNaviTitle = webNaviTitle.copy;
        }
        
        if ([self colorFromHexString:viewModelDict[@"webNaviBgColor"]]) {
            viewModel.webNaviBgColor = [self colorFromHexString:viewModelDict[@"webNaviBgColor"]];
        }
        
        // *************** Hint *************** //
        if (viewModelDict[@"notCheckProtocolHint"]) {
            viewModel.notCheckProtocolHint = viewModelDict[@"notCheckProtocolHint"];
        }
        
        // *************** UIModalPresentationStyle *************** //
        if (viewModelDict[@"modalPresentationStyle"]) {
            viewModel.modalPresentationStyle = (UIModalPresentationStyle)[viewModelDict[@"modalPresentationStyle"] integerValue];
        }
        
        // *************** OLPullAuthVCStyle *************** //
        if (viewModelDict[@"pullAuthVCStyle"]) {
            viewModel.pullAuthVCStyle = (OLPullAuthVCStyle)[viewModelDict[@"pullAuthVCStyle"] integerValue];
        }
        
        // *************** UIUserInterfaceStyle *************** //
        if (viewModelDict[@"userInterfaceStyle"]) {
            viewModel.userInterfaceStyle = [viewModelDict[@"userInterfaceStyle"] isKindOfClass:[NSNumber class]] ? viewModelDict[@"userInterfaceStyle"] : [NSNumber numberWithInteger:[viewModelDict[@"userInterfaceStyle"] integerValue]];
        }
        
        // *************** block *************** //
        
        // widgets
        NSArray *tempWidgets = widgets.copy;
        if ((nil == tempWidgets || 0 == tempWidgets.count) && viewModelDict[@"widgets"]) {
            tempWidgets = viewModelDict[@"widgets"];
        }
        
        if ([tempWidgets isKindOfClass:[NSArray class]] && tempWidgets.count > 0) {
            viewModel.customUIHandler = ^(UIView * _Nonnull customAreaView) {
                for (NSInteger i = 0; i < tempWidgets.count; i++) {
                    NSDictionary *widgetDict = nil;
                    if ([tempWidgets[i] isKindOfClass:[NSDictionary class]]) {
                        widgetDict = tempWidgets[i];
                    } else if ([self isValidString:tempWidgets[i]]) {
                        NSError *jsonError = nil;
                        widgetDict = [NSJSONSerialization JSONObjectWithData:[tempWidgets[i] dataUsingEncoding:NSUTF8StringEncoding] options:(NSJSONReadingOptions)0 error:&jsonError];
                    }
                    NSLog(@"widgetDict:%@",widgetDict);
                    UIView *view = [self widgetFromDict:widgetDict];
                    NSLog(@"widgetFromDict:%@",view);
                    if (view && !CGRectEqualToRect(CGRectZero, view.frame)) {
                        [customAreaView addSubview:view];
                    }
                }
            };
        }
        
//        __weak typeof(self) wself = self;
        if (viewModelDict[@"authVCTransitionBlock"]) {
            viewModel.authVCTransitionBlock = ^(CGSize size, id<UIViewControllerTransitionCoordinator>  _Nonnull coordinator, UIView * _Nonnull customAreaView) {
                [self unitySendMessage:self.objName method:viewModelDict[@"authVCTransitionBlock"] msgDict:nil];
            };
        }
        
        if (viewModelDict[@"tapAuthBackgroundBlock"]) {
            viewModel.tapAuthBackgroundBlock = ^{
                [self unitySendMessage:self.objName method:viewModelDict[@"tapAuthBackgroundBlock"] msgDict:nil];
            };
        }
        
        if (viewModelDict[@"viewLifeCycleBlock"]) {
            viewModel.viewLifeCycleBlock = ^(NSString * _Nonnull viewLifeCycle, BOOL animated) {
                [self unitySendMessage:self.objName method:viewModelDict[@"viewLifeCycleBlock"] msgDict:@{@"viewLifeCycle" : viewLifeCycle}];
            };
        }
        
        if (viewModelDict[@"clickBackButtonBlock"]) {
            viewModel.clickBackButtonBlock = ^{
                [self unitySendMessage:self.objName method:viewModelDict[@"clickBackButtonBlock"] msgDict:nil];
            };
        }
        
        if (viewModelDict[@"clickSwitchButtonBlock"]) {
            viewModel.clickSwitchButtonBlock = ^{
                [self unitySendMessage:self.objName method:viewModelDict[@"clickSwitchButtonBlock"] msgDict:nil];
            };
        }
        
        if (viewModelDict[@"clickCheckboxBlock"]) {
            viewModel.clickCheckboxBlock = ^(BOOL isChecked) {
                [self unitySendMessage:self.objName method:viewModelDict[@"clickCheckboxBlock"] msgDict:@{@"isChecked" : (isChecked ? @"true" : @"false")}];
            };
        }
    }
    
    [OneLoginPro requestTokenWithViewController:[self findCurrentShowingViewController] viewModel:viewModel completion:^(NSDictionary * _Nullable result) {
        [self unitySendMessage:self.objName method:self.requestTokenCallbackName msgDict:result];
    }];
}

- (void)dismissAuthViewController {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    NSLog(@"============ dismissAuthViewController ==============");
    [OneLoginPro dismissAuthViewController:nil];
}

- (void)setLogEnabled:(BOOL)enabled {
    [OneLoginPro setLogEnabled:enabled];
}

- (void)renewPreGetToken {
    [OneLoginPro renewPreGetToken];
}

- (BOOL)isPreGetTokenResultValidate {
    return [OneLoginPro isPreGetTokenResultValidate];
}

- (void)setRequestTimeout:(NSTimeInterval)timeout {
    [OneLoginPro setRequestTimeout:timeout];
}

- (NSString *)sdkVersion {
    return [OneLoginPro sdkVersion];
}

- (void)registerOnepassCallback:(NSString *)objName verifyPhoneCallbackName:(NSString *)verifyPhoneCallbackName validatePhoneCallbackName:(NSString *)validatePhoneCallbackName {
    NSLog(@"============ register onepass callback ==============");
    
    self.objName = objName;
    self.verifyPhoneCallbackName = verifyPhoneCallbackName;
    self.validatePhoneCallbackName = validatePhoneCallbackName;
}

- (void)initWithCustiomId:(NSString * _Nonnull)customID timeout:(NSTimeInterval)timeout {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    self.gopManager = [[GOPManager alloc] initWithCustomID:customID timeout:timeout];
    self.gopManager.delegate = self;
}

- (void)verifyPhoneNumber:(NSString *)phoneNumber {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    [self.gopManager verifyPhoneNumber:phoneNumber];
}

// MARK: GOPManagerDelegate

- (void)gtOnePass:(GOPManager *)manager didReceiveDataToVerify:(NSDictionary *)data {
    [self unitySendMessage:self.objName method:self.verifyPhoneCallbackName msgDict:data];
}

- (void)gtOnePass:(GOPManager *)manager errorHandler:(GOPError *)error {
    [self unitySendMessage:self.objName method:self.verifyPhoneCallbackName msgDict:@{@"errorMsg":error.description ?: @"onepass failed"}];
}

// MARK: Validate Token

- (void)validateToken:(NSString *)token appID:(NSString *)appID processID:(NSString *)processID authcode:(NSString *)authcode {
    NSString *oneloginResult = @"onelogin/result";
    NSURL *url = [NSURL URLWithString:[NSString stringWithFormat:@"%@%@", @"http://onepass.geetest.com/", oneloginResult]];
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:url];
    request.HTTPMethod = @"POST";
    NSMutableDictionary *params = @{}.mutableCopy;
    if (token) {
        params[@"token"] = token;
    }
    if (appID) {
        params[@"id_2_sign"] = appID;
    }
    if (processID) {
        params[@"process_id"] = processID;
    }
    if (authcode) {
        params[@"authcode"] = authcode;
    }
    request.HTTPBody = [NSJSONSerialization dataWithJSONObject:params options:NSJSONWritingPrettyPrinted error:nil];
    NSURLSession *session = [NSURLSession sessionWithConfiguration:[NSURLSessionConfiguration defaultSessionConfiguration]];
    NSURLSessionDataTask *task = [session dataTaskWithRequest:request completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error) {
        id result = nil;
        if (data && !error) {
            result = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers error:nil];
        }
        [self finishValidatingToken:result error:error];
        
        if (![result isKindOfClass:[NSDictionary class]] && nil != data) {
            NSLog(@"validateToken result: %@", [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding]);
        }
    }];
    [task resume];
}

- (void)finishValidatingToken:(NSDictionary *)result error:(NSError *)error {
    NSLog(@"validateToken result: %@, error: %@", result, error);
    dispatch_async(dispatch_get_main_queue(), ^{
        [self unitySendMessage:self.objName method:self.getPhoneCallbackName msgDict:result];
    });
}

// MARK: Validate Onepass Token

- (void)validateOnePassAccessCode:(NSString *)accessCode customId:(NSString *)customId processId:(NSString *)processId phone:(NSString *)phone operatorType:(NSString *)operatorType {
    NSMutableDictionary *params = [NSMutableDictionary dictionary];
    if ([self isValidString:accessCode]) {
        params[@"accesscode"] = accessCode;
    }
    if ([self isValidString:customId]) {
        params[@"id_2_sign"] = customId;
    }
    if ([self isValidString:processId]) {
        params[@"process_id"] = processId;
    }
    if ([self isValidString:operatorType]) {
        params[@"operatorType"] = operatorType;
    }
    if ([self isValidString:phone]) {
        params[@"phone"] = phone;
    }
    NSURL *url = [NSURL URLWithString:@"http://onepass.geetest.com/v2.0/result"];
    NSMutableURLRequest *req = [NSMutableURLRequest requestWithURL:url];
    req.HTTPMethod = @"POST";
    req.HTTPBody = [NSJSONSerialization dataWithJSONObject:params options:0 error:nil];
    NSURLSessionDataTask *task = [NSURLSession.sharedSession dataTaskWithRequest:req completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error) {
        NSLog(@"verify onepass result: %@", [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding]);
        dispatch_async(dispatch_get_main_queue(), ^{
            if (nil != data) {
                NSDictionary *result = [NSJSONSerialization JSONObjectWithData:data options:(NSJSONReadingOptions)0 error:nil];
                if (result[@"status"] && [@(200) isEqual:result[@"status"]]) {
                    if (result[@"result"] && [@"0" isEqual:result[@"result"]]) {
                        [self unitySendMessage:self.objName method:self.validatePhoneCallbackName msgDict:result];
                    } else if (result[@"result"] && [@"1" isEqual:result[@"result"]]) {
                        [self unitySendMessage:self.objName method:self.validatePhoneCallbackName msgDict:result];
                    } else {
                        [self verifyOnepassFailed];
                    }
                } else {
                    [self verifyOnepassFailed];
                }
            } else {
                [self verifyOnepassFailed];
            }
        });
    }];
    [task resume];
}

- (void)verifyOnepassFailed {
    [self unitySendMessage:self.objName method:self.validatePhoneCallbackName msgDict:@{@"errorMsg" : @"validate phone failed"}];
}

// MARK: Send Unity3D Message

- (void)unitySendMessage:(NSString *)obj method:(NSString *)method msgDict:(NSDictionary *)msgDict {
    NSString *params = @"";
    if (nil != msgDict && [msgDict isKindOfClass:[NSDictionary class]] && msgDict.count > 0) {
        NSError *jsonError = nil;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:msgDict options:(NSJSONWritingOptions)0 error:&jsonError];
        if (nil == jsonError && [jsonData isKindOfClass:[NSData class]] && jsonData.length > 0) {
            params = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        }
    }
    UnitySendMessage([self isValidString:obj] ? [obj UTF8String] : [@"" UTF8String], [self isValidString:method] ? [method UTF8String] : [@"" UTF8String], [params UTF8String]);
}

// MARK: Show Alert

- (void)showAlertMessage:(NSString *)message {
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.8 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
        UIAlertController *controller = [UIAlertController alertControllerWithTitle:nil message:message preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *okAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            [controller dismissViewControllerAnimated:YES completion:nil];
        }];
        [controller addAction:okAction];
        [[self findCurrentShowingViewController] presentViewController:controller animated:YES completion:nil];
    });
}

// MARK: Rect From String

- (OLRect)rectFromString:(NSString *)string {
    if ([self isValidString:string]) {
        NSArray *rectArray = [string componentsSeparatedByString:@","];
        if (rectArray.count >= 8) {
            OLRect rect = {(CGFloat)[rectArray[0] doubleValue], (CGFloat)[rectArray[1] doubleValue], (CGFloat)[rectArray[2] doubleValue], (CGFloat)[rectArray[3] doubleValue], (CGFloat)[rectArray[4] doubleValue], (CGFloat)[rectArray[5] doubleValue], {(CGFloat)[rectArray[6] doubleValue], (CGFloat)[rectArray[7] doubleValue]}};
            return rect;
        }
    }
    
    OLRect rectZero = {0, 0, 0, 0, 0, 0, {0, 0}};
    return rectZero;
}

- (BOOL)isEqualToZeroOLRect:(OLRect)rect {
    double value = 0.000001;
    return (fabs(rect.portraitTopYOffset) < value &&
            fabs(rect.portraitLeftXOffset) < value &&
            fabs(rect.portraitCenterXOffset) < value &&
            fabs(rect.landscapeTopYOffset) < value &&
            fabs(rect.landscapeLeftXOffset) < value &&
            fabs(rect.landscapeCenterXOffset) < value &&
            fabs(rect.size.width) < value &&
            fabs(rect.size.height) < value);
}

// MARK: Font From String

- (UIFont *)fontFromString:(id)string {
    return [self fontFromString:string isBold:NO];
}

- (UIFont *)fontFromString:(id)string isBold:(BOOL)isBold {
    return [self fontFromString:string fontname:nil isBold:isBold];
}

- (UIFont *)fontFromString:(id)fontsize fontname:(NSString *)fontname isBold:(BOOL)isBold {
    if ([self isValidString:fontsize] || [fontsize isKindOfClass:[NSNumber class]]) {
        if ([fontsize doubleValue] > 0) {
            if ([self isValidString:fontname]) {
                return [UIFont fontWithName:fontname size:[fontsize doubleValue]];
            }
            return isBold ? [UIFont boldSystemFontOfSize:[fontsize doubleValue]] : [UIFont systemFontOfSize:[fontsize doubleValue]];
        }
    }
    
    return nil;
}

// MARK: Color From Hex

- (UIColor *)colorFromHexString:(const NSString *)hexString {
    if ([hexString isKindOfClass:[NSString class]] && hexString.length > 0) {
        NSString *tmpHexString = hexString.copy;
        if ([tmpHexString hasPrefix:@"#"]) {
            tmpHexString = [tmpHexString substringFromIndex:[@"#" length]];
        }
        
        if (tmpHexString.length > 0) {
            if (tmpHexString.length >= 8) {             // 大于8位，取前8位
                tmpHexString = [tmpHexString substringToIndex:8];
            } else if (tmpHexString.length >= 6) {      // 大于6位，取前6位
                tmpHexString = [tmpHexString substringToIndex:6];
            } else {                                    // 不足6位，前面补0
                while (tmpHexString.length < 6) {
                    tmpHexString = [@"0" stringByAppendingString:tmpHexString];
                }
            }
            
            NSRange range = NSMakeRange(0, 2);
            NSString *aString = nil;
            if (8 == tmpHexString.length) {
                aString = [tmpHexString substringWithRange:range];
                range.location = range.location + 2;
            }
            NSString *rString = [tmpHexString substringWithRange:range];
            range.location = range.location + 2;
            NSString *gString = [tmpHexString substringWithRange:range];
            range.location = range.location + 2;
            NSString *bString = [tmpHexString substringWithRange:range];
            // 取三种颜色值
            unsigned int r, g, b;
            [[NSScanner scannerWithString:rString] scanHexInt:&r];
            [[NSScanner scannerWithString:gString] scanHexInt:&g];
            [[NSScanner scannerWithString:bString] scanHexInt:&b];
            unsigned int a = 255;
            if (aString.length > 0) {
                [[NSScanner scannerWithString:aString] scanHexInt:&a];
            }
            return [UIColor colorWithRed:((float)r / 255.0f)
                                   green:((float)g / 255.0f)
                                    blue:((float)b / 255.0f)
                                   alpha:((float)a / 255.0f)];
        }
    }
    
    return nil;
}

// MARK: Actions

- (void)customButtonAction:(GTButton *)button {
    if ([self isValidString:button.buttonAction]) {
        [self unitySendMessage:self.objName method:button.buttonAction msgDict:nil];
    }
}

// MARK: Widget

- (UIView *)widgetFromDict:(NSDictionary *)widgetDict {
    if (widgetDict[@"type"]) {
        NSString *widgetType = widgetDict[@"type"];
        if ([widgetType isEqualToString:@"UIButton"]) {
            GTButton *button = [GTButton buttonWithType:widgetDict[@"UIButtonType"] ? (UIButtonType)[widgetDict[@"UIButtonType"] integerValue] : UIButtonTypeCustom];
            if ([self imageWithName:widgetDict[@"image"]]) {
                [button setImage:[self imageWithName:widgetDict[@"image"]] forState:UIControlStateNormal];
            }
            if ([self imageWithName:widgetDict[@"backgroundImage"]]) {
                [button setBackgroundImage:[self imageWithName:widgetDict[@"backgroundImage"]] forState:UIControlStateNormal];
            }
            if ([self isValidString:widgetDict[@"title"]]) {
                [button setTitle:widgetDict[@"title"] forState:UIControlStateNormal];
            }
            if (widgetDict[@"titleColor"]) {
                [button setTitleColor:[self colorFromHexString:widgetDict[@"titleColor"]] forState:UIControlStateNormal];
            }
            if ([self fontFromString:widgetDict[@"titleFont"]]) {
                button.titleLabel.font = [self fontFromString:widgetDict[@"titleFont"]];
            }
            if (widgetDict[@"cornerRadius"]) {
                button.layer.masksToBounds = YES;
                button.layer.cornerRadius = [widgetDict[@"cornerRadius"] doubleValue];
            }
            if (widgetDict[@"action"]) {
                button.buttonAction = widgetDict[@"action"];
                [button addTarget:self action:@selector(customButtonAction:) forControlEvents:UIControlEventTouchUpInside];
            }
            if (widgetDict[@"frame"]) {
                button.frame = [self frameFromArray:widgetDict[@"frame"]];
            }
            return button;
        } else if ([widgetType isEqualToString:@"UILabel"]) {
            UILabel *label = [UILabel new];
            if ([self colorFromHexString:widgetDict[@"backgroundColor"]]) {
                label.backgroundColor = [self colorFromHexString:widgetDict[@"backgroundColor"]];
            }
            if ([self colorFromHexString:widgetDict[@"textColor"]]) {
                label.textColor = [self colorFromHexString:widgetDict[@"textColor"]];
            }
            if ([self fontFromString:widgetDict[@"font"]]) {
                label.font = [self fontFromString:widgetDict[@"font"]];
            }
            if (widgetDict[@"textAlignment"]) {
                label.textAlignment = (NSTextAlignment)[widgetDict[@"textAlignment"] integerValue];
            }
            if (widgetDict[@"frame"]) {
                label.frame = [self frameFromArray:widgetDict[@"frame"]];
            }
            if ([self isValidString:widgetDict[@"text"]]) {
                label.text = widgetDict[@"text"];
            }
            if (widgetDict[@"cornerRadius"]) {
                label.layer.masksToBounds = YES;
                label.layer.cornerRadius = [widgetDict[@"cornerRadius"] doubleValue];
            }
            return label;
        } else if ([widgetType isEqualToString:@"UIView"]) {
            UIView *view = [UIView new];
            if ([self colorFromHexString:widgetDict[@"backgroundColor"]]) {
                view.backgroundColor = [self colorFromHexString:widgetDict[@"backgroundColor"]];
            }
            if (widgetDict[@"frame"]) {
                view.frame = [self frameFromArray:widgetDict[@"frame"]];
            }
            if (widgetDict[@"cornerRadius"]) {
                view.layer.masksToBounds = YES;
                view.layer.cornerRadius = [widgetDict[@"cornerRadius"] doubleValue];
            }
            return view;
        } else if ([widgetType isEqualToString:@"UITextField"]) {
            UITextField *textField = [UITextField new];
            if ([self colorFromHexString:widgetDict[@"backgroundColor"]]) {
                textField.backgroundColor = [self colorFromHexString:widgetDict[@"backgroundColor"]];
            }
            if ([self colorFromHexString:widgetDict[@"textColor"]]) {
                textField.textColor = [self colorFromHexString:widgetDict[@"textColor"]];
            }
            if ([self fontFromString:widgetDict[@"font"]]) {
                textField.font = [self fontFromString:widgetDict[@"font"]];
            }
            if ([self isValidString:widgetDict[@"placeholder"]]) {
                textField.placeholder = widgetDict[@"placeholder"];
            }
            if (widgetDict[@"textAlignment"]) {
                textField.textAlignment = (NSTextAlignment)[widgetDict[@"textAlignment"] integerValue];
            }
            if (widgetDict[@"frame"]) {
                textField.frame = [self frameFromArray:widgetDict[@"frame"]];
            }
            if (widgetDict[@"cornerRadius"]) {
                textField.layer.masksToBounds = YES;
                textField.layer.cornerRadius = [widgetDict[@"cornerRadius"] doubleValue];
            }
            return textField;
        } else if ([widgetType isEqualToString:@"UITextView"]) {
            UITextView *textView = [UITextView new];
            if ([self colorFromHexString:widgetDict[@"backgroundColor"]]) {
                textView.backgroundColor = [self colorFromHexString:widgetDict[@"backgroundColor"]];
            }
            if ([self colorFromHexString:widgetDict[@"textColor"]]) {
                textView.textColor = [self colorFromHexString:widgetDict[@"textColor"]];
            }
            if ([self fontFromString:widgetDict[@"font"]]) {
                textView.font = [self fontFromString:widgetDict[@"font"]];
            }
            if (widgetDict[@"textAlignment"]) {
                textView.textAlignment = (NSTextAlignment)[widgetDict[@"textAlignment"] integerValue];
            }
            if (widgetDict[@"frame"]) {
                textView.frame = [self frameFromArray:widgetDict[@"frame"]];
            }
            if (widgetDict[@"cornerRadius"]) {
                textView.layer.masksToBounds = YES;
                textView.layer.cornerRadius = [widgetDict[@"cornerRadius"] doubleValue];
            }
            return textView;
        } else if ([widgetType isEqualToString:@"UIImageView"]) {
            UIImageView *imageView = [UIImageView new];
            if ([self imageWithName:widgetDict[@"image"]]) {
                imageView.image = [self imageWithName:widgetDict[@"image"]];
            }
            if ([self colorFromHexString:widgetDict[@"backgroundColor"]]) {
                imageView.backgroundColor = [self colorFromHexString:widgetDict[@"backgroundColor"]];
            }
            if (widgetDict[@"frame"]) {
                imageView.frame = [self frameFromArray:widgetDict[@"frame"]];
            }
            if (widgetDict[@"cornerRadius"]) {
                imageView.layer.masksToBounds = YES;
                imageView.layer.cornerRadius = [widgetDict[@"cornerRadius"] doubleValue];
            }
            return imageView;
        }
    }
    
    return nil;
}

- (CGRect)frameFromArray:(id)array {
    if ([array isKindOfClass:[NSArray class]] && ((NSArray *)array).count >= 4) {
        CGRect frame = {{(CGFloat)[array[0] doubleValue], (CGFloat)[array[1] doubleValue]}, {(CGFloat)[array[2] doubleValue], (CGFloat)[array[3] doubleValue]}};
        return frame;
    } else if ([self isValidString:array]) {
        NSArray *frameArray = [array componentsSeparatedByString:@","];
        if ([frameArray isKindOfClass:[NSArray class]] && frameArray.count >= 4) {
            return [self frameFromArray:frameArray];
        }
    }
    
    return CGRectZero;
}

// MARK: Find Current ViewController

// 获取当前显示的 UIViewController
- (UIViewController *)findCurrentShowingViewController {
    //获得当前活动窗口的根视图
    UIViewController *vc = [UIApplication sharedApplication].keyWindow.rootViewController;
    UIViewController *currentShowingVC = [self findCurrentShowingViewControllerFrom:vc];
    return currentShowingVC;
}

- (UIViewController *)findCurrentShowingViewControllerFrom:(UIViewController *)vc {
    // 递归方法 Recursive method
    UIViewController *currentShowingVC;
    if ([vc presentedViewController]) {
        // 当前视图是被presented出来的
        UIViewController *nextRootVC = [vc presentedViewController];
        currentShowingVC = [self findCurrentShowingViewControllerFrom:nextRootVC];

    } else if ([vc isKindOfClass:[UITabBarController class]]) {
        // 根视图为UITabBarController
        UIViewController *nextRootVC = [(UITabBarController *)vc selectedViewController];
        currentShowingVC = [self findCurrentShowingViewControllerFrom:nextRootVC];

    } else if ([vc isKindOfClass:[UINavigationController class]]){
        // 根视图为UINavigationController
        UIViewController *nextRootVC = [(UINavigationController *)vc visibleViewController];
        currentShowingVC = [self findCurrentShowingViewControllerFrom:nextRootVC];

    } else {
        // 根视图为非导航类
        currentShowingVC = vc;
    }

    return currentShowingVC;
}

// MARK: Image

- (UIImage * _Nullable)imageWithName:(NSString *)name {
    if ([self isValidString:name]) {
        return [UIImage imageNamed:name];
    }
    
    return nil;
}

// MARK: Valid String

- (BOOL)isValidString:(NSString *)string {
    return string && [string isKindOfClass:[NSString class]] && string.length > 0 && ![string isEqual:@"NSNull"];
}

// MARK: C Sharp Methods

#if defined(__cplusplus)
extern "C"{
#endif

    char* getCurrentCarrier() {
        return strdup([[UnityPlugin getCurrentCarrier] UTF8String]);
    }

    void setProtocolCheckState(bool isChecked) {
        [UnityPlugin setProtocolCheckState:isChecked];
    }
    void deletePreResultCache() {
        [UnityPlugin deletePreResultCache];
    }

    void registerWihtAppID(const char *appId) {
        [UnityPlugin registerWihtAppID:[NSString stringWithUTF8String:appId]];
    }
    
    void registerCallback(const char *objName, const char *requestTokenCallbackName, const char *getPhoneCallbackName) {
        [UnityPlugin registerCallbackWithObjName:[NSString stringWithUTF8String:objName] requestTokenCallbackName:[NSString stringWithUTF8String:requestTokenCallbackName] getPhoneCallbackName:[NSString stringWithUTF8String:getPhoneCallbackName]];
    }
    
    void enterAuthController(const char *configs, char **widgets) {
        NSMutableArray *tempWidgets = [NSMutableArray new];
        if (NULL != widgets && NULL != *widgets) {
            int widgetLen = 0;
            char *temp = widgets[0];
            while (temp) {
                widgetLen++;
                temp = widgets[widgetLen];
            }
            for (int i = 0; i < widgetLen; i++) {
                [tempWidgets addObject:[NSString stringWithUTF8String:widgets[i]]];
            }
        }
        [UnityPlugin enterAuthViewController:[NSString stringWithUTF8String:configs] widgets:tempWidgets.copy];
    }
    
    void dismissAuthViewController() {
        [UnityPlugin dismissAuthViewController];
    }
    
    void setLogEnabled(bool enabled) {
        [UnityPlugin setLogEnabled:enabled];
    }
    
    void renewPreGetToken() {
        [UnityPlugin renewPreGetToken];
    }
    
    bool isPreGetTokenResultValidate() {
        return [UnityPlugin isPreGetTokenResultValidate];
    }
    
    void setRequestTimeout(double timeout) {
        [UnityPlugin setRequestTimeout:timeout];
    }
    
    char* sdkVersion() {
        return strdup([[UnityPlugin sdkVersion] UTF8String]);
    }
    
    void registerOnepassCallback(const char *objName, const char *verifyPhoneCallbackName, const char *validatePhoneCallbackName) {
        [UnityPlugin registerOnepassCallback:[NSString stringWithUTF8String:objName] verifyPhoneCallbackName:[NSString stringWithUTF8String:verifyPhoneCallbackName] validatePhoneCallbackName:[NSString stringWithUTF8String:validatePhoneCallbackName]];
    }
    
    void initOnePass(const char *customID, double timeout) {
        [UnityPlugin initWithCustiomId:[NSString stringWithUTF8String:customID] timeout:timeout];
    }
    
    void verifyPhoneNumber(const char *phoneNumber) {
        [UnityPlugin verifyPhoneNumber:[NSString stringWithUTF8String:phoneNumber]];
    }
    
    void validateOnePassAccessCode(const char *accessCode, const char *customId, const char *processId, const char *phone, const char *operatorType) {
        [UnityPlugin validateOnePassAccessCode:[NSString stringWithUTF8String:accessCode] customId:[NSString stringWithUTF8String:customId] processId:[NSString stringWithUTF8String:processId] phone:[NSString stringWithUTF8String:phone] operatorType:[NSString stringWithUTF8String:operatorType]];
    }
    
    void validateToken(const char *token, const char *appID, const char *processID, const char *authcode) {
        [UnityPlugin validateToken:[NSString stringWithUTF8String:token] appID:[NSString stringWithUTF8String:appID] processID:[NSString stringWithUTF8String:processID] authcode:[NSString stringWithUTF8String:authcode]];
    }
    
    void showAlertMessage(const char *message) {
        [UnityPlugin showAlertMessage:[NSString stringWithUTF8String:message]];
    }
#if defined(__cplusplus)
}
#endif

@end
